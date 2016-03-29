namespace Yakari
{
    public interface ICacheCollectionObserver
    {
        void BeforeAdd(string key, InMemoryCacheItem item);

        void AfterAdd(string key, InMemoryCacheItem item);

        void BeforeUpdate(string key, InMemoryCacheItem item);

        void AfterUpdate(string key, InMemoryCacheItem item);

        void BeforeDelete(string key);

        void AfterDelete(string key);

        void BeforeGet(string key);

        void AfterGet(string key);
    }
}