using System;

namespace Yakari.RedisClient
{
    public class RedisCacheObserver: ICacheObserver
    {
        public RedisCacheObserver(ICacheProvider provider)
        {
            
        }

        public void BeforeSet(string key, InMemoryCacheItem item)
        {
            throw new NotImplementedException();
        }

        public void AfterSet(string key, InMemoryCacheItem item)
        {
            throw new NotImplementedException();
        }

        public void BeforeDelete(string key)
        {
            throw new NotImplementedException();
        }

        public void AfterDelete(string key)
        {
            throw new NotImplementedException();
        }

        public void BeforeGet(string key)
        {
            throw new NotImplementedException();
        }

        public void AfterGet(string key)
        {
            throw new NotImplementedException();
        }
    }
}
