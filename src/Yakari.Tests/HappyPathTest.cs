using System;
using System.Collections.Generic;
using NUnit.Framework;
using Yakari.Demo;

namespace Yakari.Tests
{
    [TestFixture]
    public class HappyPathTest
    {
         
    }

    public class HappyPather
    {
        private readonly DependencyContainer _container;
        private IDemoHelper _demoHelper;
        private ICacheProvider _localCache;
        private ICacheProvider _remoteCache;

        public HappyPather(DependencyContainer container)
        {
            _container = container;
            _demoHelper = _container.Resolve<IDemoHelper>();
            _localCache = _container.Resolve<ICacheProvider>(DependencyContainer.LocalCacheProviderName);
            _remoteCache = _container.Resolve<ICacheProvider>(DependencyContainer.RemoteCacheProviderName);
        }

        public List<DemoObject> FillSomeDemoObjectToLocalCache(string key)
        {
            var list = _demoHelper.GenerateDemoObjects(1000);
            _localCache.Set(key, list, CacheTime.FifteenMinutes);
            return list;
        }

        public List<DemoObject> GetSomeDemoObjectFromLocal(string key)
        {
            var list = _localCache.Get<List<DemoObject>>(key, TimeSpan.FromSeconds(3));
            return list;
        }

        public InMemoryCacheItem GetSomeDemoObjectFromRemote(string key)
        {
            var item = _remoteCache.Get<InMemoryCacheItem>(key, TimeSpan.FromSeconds(3));
            return item;
        }

    }
}