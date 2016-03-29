using System;

namespace Yakari
{
    public interface ICacheProvider : IDisposable
    {
        bool HasSlidingSupport { get; }

        /// <summary>
        ///     Get cache item from cache store
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>(string key) where T : class;

        /// <summary>
        ///     Get cache item from cache store
        /// </summary>
        /// <param name="key"></param>
        /// <param name="acquireFunction">Action to get data from store</param>
        /// <param name="expiresIn"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>(string key, Func<T> acquireFunction, TimeSpan expiresIn) where T : class;

        /// <summary>
        ///     Set generic cache item with expiration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        void Set<T>(string key, T value, TimeSpan expiresIn) where T : class;

        /// <summary>
        ///     Delete cache item
        /// </summary>
        /// <param name="key"></param>
        void Delete(string key);
    }
}