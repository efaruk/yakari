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
        /// <param name="timeOut"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>(string key, TimeSpan timeOut) where T : class;

        /// <summary>
        ///     Get cache item from cache store
        /// </summary>
        /// <param name="key"></param>
        /// <param name="getTimeOut"></param>
        /// <param name="acquireFunction">Action to get data from store</param>
        /// <param name="expiresIn"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>(string key, TimeSpan getTimeOut, Func<T> acquireFunction, TimeSpan expiresIn) where T : class;

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
    }
}