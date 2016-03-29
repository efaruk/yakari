using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Yakari
{
    public class ObserveableConcurrentCacheCollection: IDisposable
    {
        private readonly ICacheProviderOptions _options;

        public ObserveableConcurrentCacheCollection(ICacheProviderOptions options)
        {
            _options = options;
            _concurrentCollection = new ConcurrentDictionary<string, InMemoryCacheItem>(_options.ConcurrencyLevel, _options.InitialCapacity);
        }
        
        public ObserveableConcurrentCacheCollection(ICacheProviderOptions options, ConcurrentDictionary<string, InMemoryCacheItem> concurrentCollection)
        {
            _options = options;
            _concurrentCollection = concurrentCollection;
        }

        private ConcurrentDictionary<string, InMemoryCacheItem> _concurrentCollection;

        public virtual void AddOrUpdate(string key, InMemoryCacheItem item)
        {
            var func = new Func<string, InMemoryCacheItem, InMemoryCacheItem>((s, cacheItem) =>
            {
                return item;
            });
            _concurrentCollection.AddOrUpdate(key, item, func);
        }

        public virtual InMemoryCacheItem GetOrAdd(string key, InMemoryCacheItem item)
        {
            return _concurrentCollection.GetOrAdd(key, item);
        }

        public virtual void Clear()
        {
            _concurrentCollection.Clear();
        }

        public virtual bool ContainsKey(string key)
        {
            return _concurrentCollection.ContainsKey(key);
        }

        public virtual IEnumerator<KeyValuePair<string, InMemoryCacheItem>> GetEnumerator()
        {
            return _concurrentCollection.GetEnumerator();
        }

        public virtual bool TryGetValue(string key, out InMemoryCacheItem item)
        {
            return _concurrentCollection.TryGetValue(key, out item);
        }

        public virtual void TryAdd(string key, InMemoryCacheItem item)
        {
            _concurrentCollection.TryAdd(key, item);
        }

        public virtual bool TryRemove(string key, out InMemoryCacheItem item)
        {
            return _concurrentCollection.TryRemove(key, out item);
        }

        public virtual bool TryUpdate(string key, InMemoryCacheItem newItem, InMemoryCacheItem oldItem)
        {
            return _concurrentCollection.TryUpdate(key, newItem, oldItem);
        }

        public void Dispose()
        {
            _concurrentCollection = null;
        }

        /// <summary>
        ///     Clean expired cache items
        /// </summary>
        public void RemoveExpiredItems()
        {
            var expiredItems = _concurrentCollection.Where(o => o.Value.IsExpired());
            foreach (var i in expiredItems)
            {
                var c = 0;
                InMemoryCacheItem outItem;
                while (!_concurrentCollection.TryRemove(i.Key, out outItem))
                {
                    c++;
                    if (c >= _options.MaxRetryForLocalOperations) break;
                }
            }
        }
    }
}