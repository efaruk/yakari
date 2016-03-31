using System;

namespace Yakari
{
    /// <summary>
    ///     Abstract base for <see cref="ICacheProvider">ICacheProvider</see>
    /// </summary>
    public abstract class BaseCacheProvider : ICacheProvider
    {
        public abstract void Dispose();

        public abstract bool HasSlidingSupport { get; }

        public abstract T Get<T>(string key, TimeSpan timeOut) where T : class;

        public T Get<T>(string key, TimeSpan getTimeOut, Func<T> acquireFunction, TimeSpan expiresIn) where T : class
        {
            var data = Get<T>(key, getTimeOut);
            if (data == null)
            {
                data = acquireFunction();
                if (data == null) return null;
                Set(key, data, expiresIn);
            }
            return data;
        }

        public abstract void Set(string key, object value, TimeSpan expiresIn);

        public abstract void Delete(string key);

        public abstract bool Exists(string key);

    }
}