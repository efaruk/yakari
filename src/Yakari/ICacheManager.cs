using System;

namespace Yakari
{
    public interface ICacheManager: IDisposable
    {
        void SetupMember(ICacheProvider localCacheProvider);

        void OnBeforeSet(string key, InMemoryCacheItem item);

        void OnAfterSet(string key, InMemoryCacheItem item);

        void OnBeforeDelete(string key);

        void OnAfterDelete(string key);

        void OnBeforeGet(string key, TimeSpan timeOut);

        void OnAfterGet(string key);

        void OnRemoteSet(string key, string memberName);

        //void OnRemoteGet(string key, string memberName);

        void OnRemoteDelete(string key, string memberName);
    }
}