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

        public abstract T Get<T>(string key) where T : class;

        public T Get<T>(string key, Func<T> acquireFunction, TimeSpan expiresIn) where T : class
        {
            var data = Get<T>(key);
            if (data == null)
            {
                data = acquireFunction();
                if (data == null) return null;
                Set(key, data, expiresIn);
            }
            return data;
        }

        public abstract void Set<T>(string key, T value, TimeSpan expiresIn) where T : class;

        public abstract void Delete(string key);
    }
}