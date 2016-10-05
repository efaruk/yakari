using System;
using System.Collections;
using System.Collections.Generic;
using Yakari.Interfaces;

namespace Yakari
{
    /// <summary>
    ///     Abstract base for <see cref="ICacheProvider">ICacheProvider</see>
    /// </summary>
    public abstract class BaseCacheProvider : ICacheProvider
    {
        public abstract void Dispose();

        public abstract bool HasSlidingSupport { get; }

        public abstract T Get<T>(string key, TimeSpan getTimeout, bool isManagerCall = false) where T : class;

        public T Get<T>(string key, TimeSpan getTimeout, Func<T> acquireFunction, TimeSpan expiresIn, bool isManagerCall = false) where T : class
        {
            var data = Get<T>(key, getTimeout, isManagerCall);
            if (data == null)
            {
                data = acquireFunction();
                if (data == null) return null;
                Set(key, data, expiresIn, isManagerCall);
            }
            return data;
        }

        public abstract void Set(string key, object value, TimeSpan expiresIn, bool isManagerCall = false);

        public abstract void Delete(string key, bool isManagerCall = false);

        public abstract bool Exists(string key);

        public abstract List<string> AllKeys();

    }
}