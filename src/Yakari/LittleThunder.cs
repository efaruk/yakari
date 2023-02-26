using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Yakari
{
    /// <summary>
    ///     LittleThunder
    /// </summary>
    public class LittleThunder : ILocalCacheProvider
    {
        private readonly ILogger _logger;
        private readonly LocalCacheProviderOptions _localCacheProviderOptions;
        private bool disposedValue;
        private readonly TaskCompletionSource<object> _completionSource = new TaskCompletionSource<object>();
        private readonly CancellationTokenSource _shutdownCancellationTokenSource = new CancellationTokenSource();
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private Dictionary<string, InMemoryCacheItem> _cache;

        public LittleThunder(ILogger<LittleThunder> logger, LocalCacheProviderOptions localCacheProviderOptions)
        {
            _logger = logger.NotNull(nameof(logger));
            _localCacheProviderOptions = localCacheProviderOptions.NotNull(nameof(localCacheProviderOptions));
            _cache = new Dictionary<string, InMemoryCacheItem>(_localCacheProviderOptions.InitialCapacity);
        }

        public bool HasSlidingSupport => true;

        public event BeforeGetAsync OnBeforeGetAsync;

        public event AfterGetAsync OnAfterGetAsync;

        public event BeforeSetAsync OnBeforeSetAsync;

        public event AfterSetAsync OnAfterSetAsync;

        public event BeforeDeleteAsync OnBeforeDeleteAsync;

        public event AfterDelete OnAfterDeleteAsync;

        public async Task<IEnumerable<string>> AllKeysAsync(CancellationToken cancellationToken = default)
        {
            string[] keys;
            await _lock.WaitAsync();
            try
            {
                keys = _cache.Keys.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(nameof(AllKeysAsync), ex);
                throw;
            }
            finally
            {
                _lock.Release();
            }
            return keys;
        }

        public async Task DeleteAsync(string key, bool isManagerCall = false, CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync();
            try
            {
                _cache.Remove(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(nameof(AllKeysAsync), ex);
                throw;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            bool exists;
            await _lock.WaitAsync();
            try
            {
                exists = _cache.ContainsKey(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(nameof(AllKeysAsync), ex);
                throw;
            }
            finally
            {
                _lock.Release();
            }
            return exists;
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            T item = default;
            await _lock.WaitAsync();
            try
            {
                var kvp = _cache[key];
                if (kvp != null)
                {
                    kvp.Hit();
                    item = kvp.ValueObject as T;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(nameof(AllKeysAsync), ex);
                throw;
            }
            finally
            {
                _lock.Release();
            }
            return item;
        }

        public async Task<T> GetAsync<T>(string key, Func<CancellationToken, Task<T>> acquireFunction, TimeSpan expiresIn, CancellationToken cancellationToken = default) where T : class
        {
            T item = default;
            await _lock.WaitAsync();
            try
            {
                var kvp = _cache[key];
                if (kvp != null)
                {
                    kvp.Hit();
                    item = kvp.ValueObject as T;
                }
                else
                {
                    item = await acquireFunction(cancellationToken);
                    var cacheItem = new InMemoryCacheItem(item, )
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(nameof(AllKeysAsync), ex);
                throw;
            }
            finally
            {
                _lock.Release();
            }
            return item;
        }

        public async Task SetAsync(string key, object value, TimeSpan expiresIn, bool isManagerCall = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~LittleThunder()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}