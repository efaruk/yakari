using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Yakari
{
    public interface ICacheProvider : IDisposable
    {
        bool HasSlidingSupport { get; }

        /// <summary>
        ///     Get cache item from cache store
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        ///     Get cache item from cache store
        /// </summary>
        /// <param name="key"></param>
        /// <param name="acquireFunction">Action to get data from store</param>
        /// <param name="expiresIn"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> GetAsync<T>(string key, Func<CancellationToken, Task<T>> acquireFunction, TimeSpan expiresIn, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        ///     Set generic cache item with expiration
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <param name="isManagerCall">Whether or not is manager calling</param>
        Task SetAsync(string key, object value, TimeSpan expiresIn, bool isManagerCall = false, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Delete cache item
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isManagerCall">Whether or not is manager calling</param>
        Task DeleteAsync(string key, bool isManagerCall = false, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Check key exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Return all cache keys
        /// </summary>
        Task<IEnumerable<string>> AllKeysAsync(CancellationToken cancellationToken = default);
    }
}