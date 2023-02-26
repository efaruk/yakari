using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Yakari
{
    public class GreatEagle : ICacheObserver
    {
        private bool disposedValue;
        private readonly ILogger<GreatEagle> _logger;
        private readonly TaskCompletionSource<object> _completionSource = new TaskCompletionSource<object>();
        private readonly CancellationTokenSource _shutdownCancellationTokenSource = new CancellationTokenSource();

        public GreatEagle(ILogger<GreatEagle> logger, string memberName, ISubscriptionManager subscriptionManager, ISerializer serializer,
            ILocalCacheProvider localCacheProvider, IRemoteCacheProvider remoteCacheProvider)
        {
            _logger = logger.NotNull(nameof(logger));
        }

        public Task OnAfterDeleteAsync(string key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task OnAfterGetAsync(string key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task OnAfterSetAsync(string key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task OnBeforeDeleteAsync(string key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task OnBeforeGetAsync(string key, TimeSpan timeout, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task OnBeforeSetAsync(string key, InMemoryCacheItem item, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task OnRemoteDeleteAsync(string key, string memberName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task OnRemoteSetAsync(string key, string memberName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var ctReg = cancellationToken.Register(() => _shutdownCancellationTokenSource.Cancel());
            _logger.LogInformation($"{nameof(LittleThunder)}: Starting");
            _ = ServeAsync(_shutdownCancellationTokenSource.Token); // Long running cancellation cancellationToken source
            _logger.LogInformation($"{nameof(LittleThunder)}: Started");
        }

        private async Task ServeAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(LittleThunder)} starting serve loop");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(1000, cancellationToken);

                    if (_cache.Count == 0)
                    {
                        continue;
                    }

                    // Manage Expired Items
                    await MonitorItemsAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation($"{nameof(LittleThunder)} loop cancelled");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error in {nameof(LittleThunder)} loop");
                }
            }

            _logger.LogInformation($"{nameof(LittleThunder)} serve loop end");
            _completionSource.TrySetResult(this);
        }

        private async Task MonitorItemsAsync(CancellationToken cancellationToken)
        {
            await _lock.WaitAsync();
            try
            {
                foreach (var kvp in _cache)
                {
                    if (kvp.Value.IsExpired)
                    {
                        _cache.Remove(kvp.Key);
                    }
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
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(LittleThunder)}: Stopping");
            _shutdownCancellationTokenSource.Cancel();
            await _completionSource.Task;
            _logger.LogInformation($"{nameof(LittleThunder)}: Stopped");
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
        // ~GreatEagle()
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