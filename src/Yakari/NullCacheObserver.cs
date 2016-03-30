namespace Yakari
{
    public class NullCacheObserver : ICacheObserver
    {
        public void BeforeSet(string key, InMemoryCacheItem item) { }

        public void AfterSet(string key, InMemoryCacheItem item) { }

        public void BeforeDelete(string key) { }

        public void AfterDelete(string key) { }

        public void BeforeGet(string key) { }

        public void AfterGet(string key) { }
    }
}