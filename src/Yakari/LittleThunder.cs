using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Yakari
{
    /// <summary>
    ///     LittleThunder
    /// </summary>
    public class LittleThunder : BaseCacheProvider, ILocalCacheProvider
    {
        private const int Thousand = 1000;
        private ILocalCacheProviderOptions _options;
        private ConcurrentDictionary<string, InMemoryCacheItem> _concurrentStore;
        private readonly Timer _timer = new Timer(Thousand);

        /// <summary>
        ///     Constructor with options as <see cref="ILocalCacheProviderOptions">ICacheProviderOptions</see>
        /// </summary>
        /// <param name="options"></param>
        public LittleThunder(ILocalCacheProviderOptions options)
        {
            _options = options;
            _options.Logger.Log(LogLevel.Trace, "LittleThunder Constructor Begin");
            //_options.Observer.SetupMember(this);
            _concurrentStore = new ConcurrentDictionary<string, InMemoryCacheItem>(_options.ConcurrencyLevel, _options.InitialCapacity);
            _timer.Elapsed += timer_Elapsed;
            _timer.Start();
            _options.Logger.Log(LogLevel.Trace, "LittleThunder Constructor End");
        }

        /// <summary>
        ///     Reset (Reset whole provider) provider with Concurrency Level and Initial Capacity parameters.
        /// </summary>
        public void Reset(ILocalCacheProviderOptions options)
        {
            // Set options
            _options = options;
            _options.Logger.Log(LogLevel.Trace, "LittleThunder Reset Begin");
            _timer.Stop();
            _concurrentStore = new ConcurrentDictionary<string, InMemoryCacheItem>(_options.ConcurrencyLevel, _options.InitialCapacity);
            _timer.Start();
            _options.Logger.Log(LogLevel.Trace, "LittleThunder Reset End");
        }

        private bool _inPeriod;

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _options.Logger.Log(LogLevel.Trace, "LittleThunder Timer Elapsed");
            if (_disposing) return;
            if (_inPeriod) return;
            _options.Logger.Log(LogLevel.Trace, "LittleThunder In Period Begin");
            try
            {
                _inPeriod = true;
                RemoveExpiredItems();
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch(Exception ex)
            {
                _options.Logger.Log("LittleThunder In Period Exception", ex);
            }
            finally
            {
                _inPeriod = false;
            }
            _options.Logger.Log(LogLevel.Trace, "LittleThunder In Period End");
        }

        /// <summary>
        ///     Clean expired cache items
        /// </summary>
        private void RemoveExpiredItems()
        {
            _options.Logger.Log(LogLevel.Trace, "LittleThunder RemoveExpiredItems Begin");
            var expiredItems = _concurrentStore.Where(o => o.Value.IsExpired()).ToArray();
            _options.Logger.Log(LogLevel.Trace, string.Format("LittleThunder Removing {0} Item(s)", expiredItems.Length));
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
            _options.Logger.Log(LogLevel.Trace, "LittleThunder RemoveExpiredItems End");
        }

        public override T Get<T>(string key, TimeSpan getTimeout, bool isManagerCall)
        {
            return GetInternal<T>(key, getTimeout, isManagerCall);
        }

        private T GetInternal<T>(string key, TimeSpan getTimeout, bool isManagerCall, bool secondCall = false)
        {
            T t = default(T);
            _options.Logger.Log(LogLevel.Trace, "LittleThunder Get");
            OnBeforeGetWrapper(key, getTimeout, isManagerCall);
            if (_concurrentStore.ContainsKey(key))
            {
                InMemoryCacheItem outItem;
                var c = 0;
                while (!_concurrentStore.TryGetValue(key, out outItem))
                {
                    c++;
                    if (!_concurrentStore.ContainsKey(key)) break;
                    if (c >= _options.MaxRetryForLocalOperations) break;
                }
                OnAfterGetWrapper(key, isManagerCall);
                if (outItem == null) return default(T);
                outItem.Hit();
                t = (T)outItem.ValueObject;
            }
            else
            {
                if (!secondCall)
                {
                    t = ThreadHelper.WaitForResult(() => GetInternal<T>(key, getTimeout, false, true), getTimeout);
                }
            }
            ThreadHelper.RunOnDifferentThread(() => OnAfterGetWrapper(key, isManagerCall), true);
            return t;
        }

        private void OnAfterGetWrapper(string key, bool isManagerCall)
        {
            if (isManagerCall) return;
            if (OnAfterGet != null) OnAfterGet(key);
        }

        private void OnBeforeGetWrapper(string key, TimeSpan getTimeout, bool isManagerCall)
        {
            if (isManagerCall) return;
            if (OnBeforeGet != null) OnBeforeGet(key, getTimeout);
        }

        public override void Set(string key, object value, TimeSpan expiresIn, bool isManagerCall)
        {
            _options.Logger.Log(LogLevel.Debug, "LittleThunder Set");
            var item = new InMemoryCacheItem(value, expiresIn);
            OnBeforeSetWrapper(key, item, isManagerCall);
            var func = new Func<string, InMemoryCacheItem, InMemoryCacheItem>((s, cacheItem) => item);
            _concurrentStore.AddOrUpdate(key, item, func);
            OnAfterSetWrapper(key, isManagerCall);
        }

        private void OnAfterSetWrapper(string key, bool isManagerCall)
        {
            if (isManagerCall) return;
            ThreadHelper.RunOnDifferentThread(() => { if (OnAfterSet != null) OnAfterSet(key); }, true);
        }

        private void OnBeforeSetWrapper(string key, InMemoryCacheItem item, bool isManagerCall)
        {
            if (isManagerCall) return;
            //ThreadHelper.RunOnDifferentThread(() => { if (OnBeforeSet != null) OnBeforeSet(key, item); }, true);
            if (OnBeforeSet != null) OnBeforeSet(key, item);
        }

        public override void Delete(string key, bool isManagerCall)
        {
            _options.Logger.Log(LogLevel.Trace, "LittleThunder Delete");
            OnBeforeDeleteWrapper(key, isManagerCall);
            if (!_concurrentStore.ContainsKey(key)) return;
            InMemoryCacheItem outItem;
            var c = 0;
            while (_concurrentStore.TryRemove(key, out outItem))
            {
                c++;
                if (!_concurrentStore.ContainsKey(key)) break;
                if (c >= _options.MaxRetryForLocalOperations) break;
            }
            OnAfterDeleteWrapper(key, isManagerCall);
        }

        private void OnAfterDeleteWrapper(string key, bool isManagerCall)
        {
            if (isManagerCall) return;
            ThreadHelper.RunOnDifferentThread(() => { if (OnAfterDelete != null) OnAfterDelete(key); }, true);
        }

        private void OnBeforeDeleteWrapper(string key, bool isManagerCall)
        {
            if (isManagerCall) return;
            ThreadHelper.RunOnDifferentThread(() => { if (OnBeforeDelete != null) OnBeforeDelete(key); }, true);
        }

        public override bool Exists(string key)
        {
            _options.Logger.Log(LogLevel.Trace, "LittleThunder Exists");
            if (_concurrentStore.ContainsKey(key)) return true;
            return false;
        }

        public override List<string> AllKeys()
        {
            return _concurrentStore.Keys.ToList();
        }

        public event BeforeGet OnBeforeGet;
        public event AfterGet OnAfterGet;
        public event BeforeSet OnBeforeSet;
        public event AfterSet OnAfterSet;
        public event BeforeDelete OnBeforeDelete;
        public event AfterDelete OnAfterDelete;

        private bool _disposing;

        public override void Dispose()
        {
            if (_disposing) return;
            _options.Logger.Log(LogLevel.Trace, "LittleThunder Disposing");
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