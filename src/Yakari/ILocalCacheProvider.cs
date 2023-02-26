using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Yakari
{
    public interface ILocalCacheProvider : ICacheProvider
    {
        #region Events

        event BeforeGetAsync OnBeforeGetAsync;

        event AfterGetAsync OnAfterGetAsync;

        event BeforeSetAsync OnBeforeSetAsync;

        event AfterSetAsync OnAfterSetAsync;

        event BeforeDeleteAsync OnBeforeDeleteAsync;

        event AfterDelete OnAfterDeleteAsync;

        #endregion Events
    }

    public delegate Task BeforeSetAsync(string key, InMemoryCacheItem item, CancellationToken cancellationToken = default);

    public delegate Task AfterSetAsync(string key, CancellationToken cancellationToken = default);

    public delegate Task BeforeDeleteAsync(string key, CancellationToken cancellationToken = default);

    public delegate Task AfterDelete(string key, CancellationToken cancellationToken = default);

    public delegate Task BeforeGetAsync(string key, TimeSpan timeout, CancellationToken cancellationToken = default);

    public delegate Task AfterGetAsync(string key, CancellationToken cancellationToken = default);
}