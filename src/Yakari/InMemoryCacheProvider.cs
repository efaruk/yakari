using System;
using System.Timers;

namespace Yakari
{
    /// <summary>
    ///     In memory cache provider
    /// </summary>
    public class InMemoryCacheProvider : BaseCacheProvider
    {
        private readonly ICacheProviderOptions _options;
        private ObserveableConcurrentCacheCollection _concurrentStore;
        private Timer timer = new Timer(1000);

        /// <summary>
        ///     Constructor with Concurrency Level and Initial Capacity
        /// </summary>
        /// <param name="options"></param>
        public InMemoryCacheProvider(ICacheProviderOptions options)
        {
            _options = options;
            if (options.ConcurrencyLevel == 0) options.ConcurrencyLevel = 10;
            if (options.ConcurrencyLevel < 2) options.ConcurrencyLevel = 2;
            if (options.InitialCapacity == 0) options.InitialCapacity = 100;
            if (options.InitialCapacity < 10) options.InitialCapacity = 10;
            _concurrentStore = new ObserveableConcurrentCacheCollection(_options);
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        /// <summary>
        ///     Reset (Reset whole provider) for this provider with Concurrency Level and Initial Capacity parameters
        /// </summary>
        public void Reset(int? concurrencyLevel, int? initialCapacity)
        {
            timer.Stop();
            timer.Dispose();
            concurrencyLevel = concurrencyLevel ?? 10;
            var level = (concurrencyLevel < 2) ? 2 : concurrencyLevel.Value;
            initialCapacity = initialCapacity ?? 100;
            var capacity = (initialCapacity < 10) ? 10 : initialCapacity.Value;
            _concurrentStore = new ObserveableConcurrentCacheCollection(_options);
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        private bool _inPeriod;

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_disposing) return;
            if (_inPeriod) return;
            try
            {
                _inPeriod = true;
                _concurrentStore.RemoveExpiredItems();
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
            finally
            {
                _inPeriod = false;
            }
        }

        public override T Get<T>(string key)
        {
            if (!_concurrentStore.Keys.Contains(key)) return default(T);
            InMemoryCacheItem outItem;
            var c = 0;
            while (!_concurrentStore.TryGetValue(key, out outItem))
            {
                c++;
                if (c >= _options.MaxRetryForLocalOperations) break;
            }
            if (outItem == null) return default(T);
            return (T)outItem.ValueObject;
        }

        public override void Set<T>(string key, T value, TimeSpan expiresIn)
        {
            var item = new InMemoryCacheItem(value, expiresIn);
            _concurrentStore.AddOrUpdate(key, item);
        }

        public override void Delete(string key)
        {
            if (!_concurrentStore.Keys.Contains(key)) return;
            InMemoryCacheItem outItem;
            var c = 0;
            while (_concurrentStore.TryRemove(key, out outItem))
            {
                c++;
                if (c >= _options.MaxRetryForLocalOperations) break;
            }
        }

        private bool _disposing;

        public override void Dispose()
        {
            if (_disposing) return;
            _disposing = true;
            timer.Stop();
            timer.Dispose();
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