using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Timers;

namespace Yakari
{
    /// <summary>
    ///     ObservableInMemoryCacheProvider
    /// </summary>
    public class LittleThunder : BaseCacheProvider
    {
        private const int Thousand = 1000;
        private ICacheProviderOptions _options;
        private ConcurrentDictionary<string, InMemoryCacheItem> _concurrentStore;
        private readonly Timer _timer = new Timer(Thousand);

        /// <summary>
        ///     Constructor with options as <see cref="ICacheProviderOptions">ICacheProviderOptions</see>
        /// </summary>
        /// <param name="options"></param>
        public LittleThunder(ICacheProviderOptions options)
        {
            _options.Logger.Log(LogLevel.Trace, "ObservableInMemoryCacheProvider Constructor Begin");
            _options = options;
            _options.Manager.SetupMember(this);
            _concurrentStore = new ConcurrentDictionary<string, InMemoryCacheItem>(_options.ConcurrencyLevel, _options.InitialCapacity);
            _timer.Elapsed += timer_Elapsed;
            _timer.Start();
            _options.Logger.Log(LogLevel.Trace, "ObservableInMemoryCacheProvider Constructor End");
        }

        /// <summary>
        ///     Reset (Reset whole provider) provider with Concurrency Level and Initial Capacity parameters.
        /// </summary>
        public void Reset(ICacheProviderOptions options)
        {
            _options.Logger.Log(LogLevel.Trace, "ObservableInMemoryCacheProvider Reset Begin");
            _timer.Stop();
            // Set options
            _options = options;
            _concurrentStore = new ConcurrentDictionary<string, InMemoryCacheItem>(_options.ConcurrencyLevel, _options.InitialCapacity);
            _timer.Start();
            _options.Logger.Log(LogLevel.Trace, "ObservableInMemoryCacheProvider Reset End");
        }

        private bool _inPeriod;

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _options.Logger.Log(LogLevel.Trace, "ObservableInMemoryCacheProvider Timer Elapsed");
            if (_disposing) return;
            if (_inPeriod) return;
            _options.Logger.Log(LogLevel.Trace, "ObservableInMemoryCacheProvider In Period Begin");
            try
            {
                _inPeriod = true;
                RemoveExpiredItems();
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch(Exception ex)
            {
                _options.Logger.Log("ObservableInMemoryCacheProvider In Period Exception", ex);
            }
            finally
            {
                _inPeriod = false;
            }
            _options.Logger.Log(LogLevel.Trace, "ObservableInMemoryCacheProvider In Period End");
        }

        /// <summary>
        ///     Clean expired cache items
        /// </summary>
        private void RemoveExpiredItems()
        {
            _options.Logger.Log(LogLevel.Trace, "ObservableInMemoryCacheProvider RemoveExpiredItems Begin");
            var expiredItems = _concurrentStore.Where(o => o.Value.IsExpired());
            foreach (var pair in expiredItems)
            {
                var c = 0;
                InMemoryCacheItem outItem;
                while (!_concurrentStore.TryRemove(pair.Key, out outItem))
                {
                    c++;
                    if (!_concurrentStore.ContainsKey(pair.Key)) break;
                    if (c >= _options.MaxRetryForLocalOperations) break;
                }
            }
            _options.Logger.Log(LogLevel.Trace, "ObservableInMemoryCacheProvider RemoveExpiredItems End");
        }

        public override T Get<T>(string key, TimeSpan timeOut)
        {
            _options.Logger.Log(LogLevel.Trace, "ObservableInMemoryCacheProvider Get");
            _options.Manager.OnBeforeGet(key, timeOut);
            if (!_concurrentStore.ContainsKey(key))
            {
                InMemoryCacheItem outItem;
                var c = 0;
                while (!_concurrentStore.TryGetValue(key, out outItem))
                {
                    c++;
                    if (!_concurrentStore.ContainsKey(key)) break;
                    if (c >= _options.MaxRetryForLocalOperations) break;
                }
                _options.Manager.OnAfterGet(key);
                if (outItem == null) return default(T);
                outItem.Hit();
                return (T)outItem.ValueObject;
            }
            ThreadHelper.RunOnDifferentThread(() => _options.Manager.OnAfterGet(key), true);
            return default(T);
        }

        public override void Set(string key, object value, TimeSpan expiresIn)
        {
            _options.Logger.Log(LogLevel.Trace, "ObservableInMemoryCacheProvider Set");
            var item = new InMemoryCacheItem(value, expiresIn);
            _options.Manager.OnBeforeSet(key, item);
            var func = new Func<string, InMemoryCacheItem, InMemoryCacheItem>((s, cacheItem) => item);
            _concurrentStore.AddOrUpdate(key, item, func);
            ThreadHelper.RunOnDifferentThread(() => _options.Manager.OnAfterSet(key, item), true);
        }

        public override void Delete(string key)
        {
            _options.Logger.Log(LogLevel.Trace, "ObservableInMemoryCacheProvider Delete");
            _options.Manager.OnBeforeDelete(key);
            ThreadHelper.RunOnDifferentThread(() => _options.Manager.OnBeforeDelete(key), true);
            if (!_concurrentStore.ContainsKey(key)) return;
            InMemoryCacheItem outItem;
            var c = 0;
            while (_concurrentStore.TryRemove(key, out outItem))
            {
                c++;
                if (!_concurrentStore.ContainsKey(key)) break;
                if (c >= _options.MaxRetryForLocalOperations) break;
            }
            ThreadHelper.RunOnDifferentThread(() => _options.Manager.OnAfterDelete(key), true);
        }

        public override bool Exists(string key)
        {
            if (_concurrentStore.ContainsKey(key)) return true;
            return false;
        }

        private bool _disposing;

        public override void Dispose()
        {
            if (_disposing) return;
            _disposing = true;
            _timer.Stop();
            _timer.Dispose();
            _concurrentStore = null;
        }

        public override bool HasSlidingSupport
        {
            get
            {
                return true;
            }
        }
    }
}