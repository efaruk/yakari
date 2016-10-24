using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using Yakari.Demo;
using Yakari;

namespace Yakari.Tests
{
    [TestFixture]
    [Ignore("This tests is a bit fragile")]
    public class HappyPathTest
    {
        [OneTimeTearDown]
        public void Flush()
        {
            
        }

        [Test]
        public void TestHappyPath()
        {
            var key = "happy_bird";
            var container = new DemoDependencyContainer(null, "Single Bird");
            var happy = new HappyPather(container);
            var list1 = happy.FillSomeDemoObjectToLocalCache(key);
            Thread.Sleep(1000);
            var list2 = happy.GetSomeDemoObjectFromLocal(key);
            Assert.AreSame(list1, list2);
            Thread.Sleep(1000);
            var item = happy.GetSomeDemoObjectFromRemote(key);
            Assert.AreEqual(list1.Count, ((List<DemoObject>)item.ValueObject).Count);
        }
    }

    public class HappyPather
    {
        readonly IDemoHelper _demoHelper;
        readonly ILocalCacheProvider _localCache;
        readonly IRemoteCacheProvider _remoteCache;

        public HappyPather(DemoDependencyContainer demoDependencyContainer)
        {
            var container = demoDependencyContainer;
            _demoHelper = container.Resolve<IDemoHelper>();
            _localCache = container.Resolve<ILocalCacheProvider>();
            _remoteCache = container.Resolve<IRemoteCacheProvider>();
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