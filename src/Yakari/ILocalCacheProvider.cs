using System;

namespace Yakari
{
    public interface ILocalCacheProvider : IDisposable
    {
        bool HasSlidingSupport { get; }

        /// <summary>
        ///     Get cache item from cache store
        /// </summary>
        /// <param name="key"></param>
        /// <param name="getTimeout"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>(string key, TimeSpan getTimeout) where T : class;

        /// <summary>
        ///     Get cache item from cache store
        /// </summary>
        /// <param name="key"></param>
        /// <param name="getTimeout"></param>
        /// <param name="acquireFunction">Action to get data from store</param>
        /// <param name="expiresIn"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>(string key, TimeSpan getTimeout, Func<T> acquireFunction, TimeSpan expiresIn) where T : class;

        /// <summary>
        ///     Set generic cache item with expiration
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        void Set(string key, object value, TimeSpan expiresIn);

        /// <summary>
        ///     Delete cache item
        /// </summary>
        /// <param name="key"></param>
        void Delete(string key);

        /// <summary>
        ///     Check key exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Exists(string key);

        event BeforeGet OnBeforeGet;
        event AfterGet OnAfterGet;
        event BeforeSet OnBeforeSet;
        event AfterSet OnAfterSet;
        event BeforeDelete OnBeforeDelete;
        event AfterDelete OnAfterDelete;
    }

    public delegate void BeforeSet(string key, InMemoryCacheItem item);

    public delegate void AfterSet(string key);

    public delegate void BeforeDelete(string key);

    public delegate void AfterDelete(string key);

    public delegate void BeforeGet(string key, TimeSpan timeout);

    public delegate void AfterGet(string key);
}