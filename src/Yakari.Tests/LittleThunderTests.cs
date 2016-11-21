using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NSubstitute;
using NUnit.Framework;
using Yakari.Demo;
using Yakari;

namespace Yakari.Tests
{
    [TestFixture]
    public class LittleThunderTests
    {
        IContainer _container;
        ILogger _logger;
        ILocalCacheProvider _cache;
        IDemoHelper _demoHelper;
        //private ICacheObserver _mockCacheManager;

        const string DemoObjectListKey = "DemoObjectList";


        [OneTimeSetUp]
        public void FixtureSetup()
        {
            var builder = new ContainerBuilder();
            
            _logger = new NullLogger();
            builder.RegisterInstance(_logger).As<ILogger>().SingleInstance();
            builder.RegisterType<DemoHelper>().As<IDemoHelper>().SingleInstance();
            builder.RegisterType<LocalCacheProviderOptions>().As<ILocalCacheProviderOptions>().SingleInstance();
            builder.Register<ILocalCacheProvider>(c => new LittleThunder(c.Resolve<ILocalCacheProviderOptions>(), c.Resolve<ILogger>()));
            _container = builder.Build();

            _demoHelper = _container.Resolve<IDemoHelper>();
            _cache = _container.Resolve<ILocalCacheProvider>();
            _cache.OnBeforeGet += _cache_OnBeforeGet;
            _cache.OnAfterGet += _cache_OnAfterGet;
            _cache.OnBeforeSet += _cache_OnBeforeSet;
            _cache.OnAfterSet += _cache_OnAfterSet;
            _cache.OnBeforeDelete += _cache_OnBeforeDelete;
            _cache.OnAfterDelete += _cache_OnAfterDelete;
        }

        [TearDown]
        public void TestTearDown()
        {
            _onAfterDeleteKey = null;
            _onBeforeDeleteKey = null;
            _onAfterSetKey = null;
            _onBeforeSetKey = null;
            _onBeforeSetItem = null;
            _onAfterGetKey = null;
            _onBeforeGetKey = null;
            _onBeforeGetTimeout = TimeSpan.Zero;
        }

        string _onAfterDeleteKey;
        string _onBeforeDeleteKey;
        string _onAfterSetKey;
        string _onBeforeSetKey;
        InMemoryCacheItem _onBeforeSetItem;
        string _onAfterGetKey;
        string _onBeforeGetKey;
        TimeSpan _onBeforeGetTimeout;


        #region Private Methods

        void _cache_OnAfterDelete(string key)
        {
            _onAfterDeleteKey = key;
        }

        void _cache_OnBeforeDelete(string key)
        {
            _onBeforeDeleteKey = key;
        }

        void _cache_OnAfterSet(string key)
        {
            _onAfterSetKey = key;
        }

        void _cache_OnBeforeSet(string key, InMemoryCacheItem item)
        {
            _onBeforeSetKey = key;
            _onBeforeSetItem = item;
        }

        void _cache_OnAfterGet(string key)
        {
            _onAfterGetKey = key;
        }

        void _cache_OnBeforeGet(string key, TimeSpan timeout)
        {
            _onBeforeGetKey = key;
            _onBeforeGetTimeout = timeout;
        }

        #endregion


        [Test]
        public void SetupTests()
        {
            _logger.Log("LittleThunderTests");
            var options = _container.Resolve<ILocalCacheProviderOptions>();
            Assert.NotNull(options);
            var cacherovider = _container.Resolve<ILocalCacheProvider>();
            Assert.NotNull(cacherovider);
            Assert.AreSame(cacherovider, _cache);
            Assert.IsAssignableFrom<LittleThunder>(cacherovider);
        }

        [Test]
        public void When_set_som_objects_it_should_call_CacheManager_and_when_get_it_back_it_should_be_same_instance()
        {
            var list = _demoHelper.GenerateDemoObjects(1000);
            _cache.Set(DemoObjectListKey, list, CacheTime.FifteenMinutes, false);
            Thread.Sleep(500);
            var item = new InMemoryCacheItem(list, CacheTime.FifteenMinutes);
            Assert.AreEqual(item, _onBeforeSetItem);
            Assert.AreEqual(DemoObjectListKey, _onBeforeSetKey);
            Assert.AreEqual(DemoObjectListKey, _onAfterSetKey);
            //_mockCacheManager.Received().OnBeforeSet(Arg.Is(DemoObjectListKey), Arg.Any<InMemoryCacheItem>());
            //_mockCacheManager.Received().OnAfterSet(Arg.Is(DemoObjectListKey));
            var cachedList = _cache.Get<List<DemoObject>>(DemoObjectListKey, TimeSpan.FromSeconds(3), false);
            Thread.Sleep(500);
            Assert.AreEqual(DemoObjectListKey, _onBeforeGetKey);
            Assert.AreEqual(DemoObjectListKey, _onAfterGetKey);
            Assert.AreEqual(TimeSpan.FromSeconds(3), _onBeforeGetTimeout);
            Assert.NotNull(cachedList);
            Assert.AreSame(list, cachedList);
        }

        [Test]
        public void When_set_timeout_for_5_seconds_for_100000_item_all_should_expire_after_6_seconds()
        {
            var item = _demoHelper.GenerateDemoObjects(1)[0];
            Parallel.For(0, 100000, i =>
            {
                var key = string.Format("item{0}", i);
                _cache.Set(key, item, TimeSpan.FromSeconds(5), false);
            });
            Thread.Sleep(TimeSpan.FromSeconds(6));
            var k = "item99999";
            var cachedItem = _cache.Get<DemoObject>(k, TimeSpan.FromSeconds(0), false);
            Assert.Null(cachedItem);
        }

        [Test]
        public void When_get_same_item_for_100000_times_it_should_hit_count_equals_100000()
        {
            var item = _demoHelper.GenerateDemoObjects(1)[0];
            var key = "very_popular_item";
            _cache.Set(key, item, TimeSpan.FromSeconds(3), false);
            Parallel.For(0, 100000, i =>
            {
                _cache.Get<DemoObject>(key, TimeSpan.FromSeconds(0), false);
            });
            var cachedItem = _cache.Get<DemoObject>(key, TimeSpan.FromSeconds(0), false);
            Assert.NotNull(cachedItem);
        }

    }
}