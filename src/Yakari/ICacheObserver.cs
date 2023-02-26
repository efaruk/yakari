using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Yakari
{
    public interface ICacheObserver : IHostedService, IDisposable
    {
        Task OnBeforeSetAsync(string key, InMemoryCacheItem item, CancellationToken cancellationToken);

        Task OnAfterSetAsync(string key, CancellationToken cancellationToken);

        Task OnBeforeDeleteAsync(string key, CancellationToken cancellationToken);

        Task OnAfterDeleteAsync(string key, CancellationToken cancellationToken);

        Task OnBeforeGetAsync(string key, TimeSpan timeout, CancellationToken cancellationToken);

        Task OnAfterGetAsync(string key, CancellationToken cancellationToken);

        Task OnRemoteSetAsync(string key, string memberName, CancellationToken cancellationToken);

        Task OnRemoteDeleteAsync(string key, string memberName, CancellationToken cancellationToken);
    }
}