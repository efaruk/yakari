using System;

namespace Yakari
{
    public class NullCacheManager : ICacheManager
    {
        public void SetupMember(ICacheProvider localCacheProvider) { }

        public void OnBeforeSet(string key, InMemoryCacheItem item) { }

        public void OnAfterSet(string key, InMemoryCacheItem item) { }

        public void OnBeforeDelete(string key) { }

        public void OnAfterDelete(string key) { }

        public void OnBeforeGet(string key, TimeSpan timeOut) { }

        public void OnAfterGet(string key) { }

        public void OnRemoteSet(string key, string memberName) { }

        public void OnRemoteDelete(string key, string memberName) { }
        public void Dispose() { }
    }
}