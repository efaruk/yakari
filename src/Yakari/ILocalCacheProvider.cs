using System;

namespace Yakari
{
    public interface ILocalCacheProvider : ICacheProvider
    {
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