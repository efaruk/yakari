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
        private readonly IDemoHelper _demoHelper;
        private readonly ILocalCacheProvider _localCache;
        private readonly IRemoteCacheProvider _remoteCache;

        public HappyPather(DependencyContainer dependencyContainer)
        {
            var container = dependencyContainer;
            _demoHelper = container.Resolve<IDemoHelper>();
            _localCache = container.Resolve<ILocalCacheProvider>();
            _remoteCache = container.Resolve<IRemoteCacheProvider>(DependencyContainer.RemoteCacheProviderName);
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
            Assert.NotNull(list);
            return list;
        }

        public InMemoryCacheItem GetSomeDemoObjectFromRemote(string key)
        {
            var item = _remoteCache.Get<InMemoryCacheItem>(key, TimeSpan.FromSeconds(3));
            Assert.NotNull(item);
            return item;
        }

    }
}