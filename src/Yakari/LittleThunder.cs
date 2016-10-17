using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Yakari
{
    /// <summary>
    ///     LittleThunder
    /// </summary>
    public class LittleThunder : BaseCacheProvider, ILocalCacheProvider
    {
        const int Thousand = 1000;
        ILocalCacheProviderOptions _options;
        ConcurrentDictionary<string, InMemoryCacheItem> _concurrentStore;
        readonly Timer _timer;

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
            _timer = new Timer(timer_Elapsed, null, Thousand, Thousand);
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
            //_timer.Stop();
            _concurrentStore = new ConcurrentDictionary<string, InMemoryCacheItem>(_options.ConcurrencyLevel, _options.InitialCapacity);
            //_timer.Start();
            _timer.Change(Thousand, Thousand);
            _options.Logger.Log(LogLevel.Trace, "LittleThunder Reset End");
        }

        bool _inPeriod;

        void timer_Elapsed(object sender)
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
        void RemoveExpiredItems()
        {
            _options.Logger.Log(LogLevel.Trace, "LittleThunder RemoveExpiredItems Begin");
            var expiredItems = _concurrentStore.Where(o => o.Value.IsExpired).ToArray();
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

        T GetInternal<T>(string key, TimeSpan getTimeout, bool isManagerCall, bool secondCall = false)
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

        void OnAfterGetWrapper(string key, bool isManagerCall)
        {
            if (isManagerCall) return;
            OnAfterGet?.Invoke(key);
        }

        void OnBeforeGetWrapper(string key, TimeSpan getTimeout, bool isManagerCall)
        {
            if (isManagerCall) return;
            OnBeforeGet?.Invoke(key, getTimeout);
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

        void OnAfterSetWrapper(string key, bool isManagerCall)
        {
            if (isManagerCall) return;
            ThreadHelper.RunOnDifferentThread(() => { OnAfterSet?.Invoke(key); }, true);
        }

        void OnBeforeSetWrapper(string key, InMemoryCacheItem item, bool isManagerCall)
        {
            if (isManagerCall) return;
            //ThreadHelper.RunOnDifferentThread(() => { if (OnBeforeSet != null) OnBeforeSet(key, item); }, true);
            OnBeforeSet?.Invoke(key, item);
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

        void OnAfterDeleteWrapper(string key, bool isManagerCall)
        {
            if (isManagerCall) return;
            ThreadHelper.RunOnDifferentThread(() => { OnAfterDelete?.Invoke(key); }, true);
        }

        void OnBeforeDeleteWrapper(string key, bool isManagerCall)
        {
            if (isManagerCall) return;
            ThreadHelper.RunOnDifferentThread(() =>
            {
                OnBeforeDelete?.Invoke(key);
            }, true);
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

        bool _disposing;

        public override void Dispose()
        {
            if (_disposing) return;
            _options.Logger.Log(LogLevel.Trace, "LittleThunder Disposing");
            _disposing = true;
            //_timer.Stop();
            _timer.Dispose();
            _concurrentStore = null;
        }

        public override bool HasSlidingSupport => true;
    }
}