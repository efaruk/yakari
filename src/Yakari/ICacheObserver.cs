using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Yakari
{
    public interface ICacheObserver : IHostedService, IDisposable
    {
        Task OnBeforeSetAsync(Key key, InMemoryCacheItem item, CancellationToken cancellationToken);

        Task OnAfterSetAsync(Key key, CancellationToken cancellationToken);

        Task OnBeforeDeleteAsync(Key key, CancellationToken cancellationToken);

        Task OnAfterDeleteAsync(Key key, CancellationToken cancellationToken);

        Task OnBeforeGetAsync(Key key, TimeSpan timeout, CancellationToken cancellationToken);

        Task OnAfterGetAsync(Key key, CancellationToken cancellationToken);

        Task OnRemoteSetAsync(Key key, string memberName, CancellationToken cancellationToken);

        Task OnRemoteDeleteAsync(Key key, string memberName, CancellationToken cancellationToken);
    }
}