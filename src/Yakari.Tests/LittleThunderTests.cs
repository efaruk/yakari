using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LightInject;
using NSubstitute;
using NUnit.Framework;
using StackExchange.Redis;
using Yakari.Demo;
using Yakari.RedisClient;
using Yakari.Serializers.Newtonsoft;

namespace Yakari.Tests
{
    [TestFixture]
    public class LittleThunderTests
    {
        private ServiceContainer _container;
        private ILogger _logger;
        private ICacheProvider _cache;
        private IDemoHelper _demoHelper;
        private ICacheManager _mockCacheManager;

        private const string DemoObjectListKey = "DemoObjectList";


        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _container = new ServiceContainer();
            _container.SetDefaultLifetime<PerContainerLifetime>();
            _container.Register<ILogger>(factory => new ConsoleLogger(LogLevel.Trace));
            _logger = _container.GetInstance<ILogger>();
            _container.Register<IDemoHelper, DemoHelper>();
            _mockCacheManager = Substitute.For<ICacheManager>();
            _container.RegisterInstance(_mockCacheManager);
            _container.Register<ILocalCacheProviderOptions>(factory => new LocalCacheProviderOptions(factory.GetInstance<ILogger>(), factory.GetInstance<ICacheManager>()));
            _container.Register<ICacheProvider, LittleThunder>();
            _demoHelper = _container.GetInstance<IDemoHelper>();
            _cache = _container.GetInstance<ICacheProvider>();
        }

        [Test]
        public void SetupTests()
        {
            var cacheManager = _container.GetInstance<ICacheManager>();
            Assert.NotNull(cacheManager);
            var options = _container.GetInstance<ILocalCacheProviderOptions>();
            Assert.NotNull(options);
            var cacherovider = _container.GetInstance<ICacheProvider>();
            Assert.NotNull(cacherovider);
            Assert.AreSame(cacherovider, _cache);
            Assert.IsAssignableFrom<LittleThunder>(cacherovider);
        }

        [Test]
        public void When_set_som_objects_it_should_call_CacheManager_and_when_get_it_back_it_should_be_same_instance()
        {
            var list = _demoHelper.GenerateDemoObjects(1000);
            _cache.Set(DemoObjectListKey, list, CacheTime.FifteenMinutes);
            _mockCacheManager.Received().OnBeforeSet(Arg.Is(DemoObjectListKey), Arg.Any<InMemoryCacheItem>());
            _mockCacheManager.Received().OnAfterSet(Arg.Is(DemoObjectListKey), Arg.Any<InMemoryCacheItem>());
            var cachedList = _cache.Get<List<DemoObject>>(DemoObjectListKey, TimeSpan.FromSeconds(3));
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
                _cache.Set(key, item, TimeSpan.FromSeconds(5));
            });
            Thread.Sleep(TimeSpan.FromSeconds(6));
            var k = "item99999";
            var cachedItem = _cache.Get<DemoObject>(k, TimeSpan.FromSeconds(0));
            Assert.Null(cachedItem);
        }

        [Test]
        public void When_get_same_item_for_100000_times_it_should_hit_count_equals_100000()
        {
            var item = _demoHelper.GenerateDemoObjects(1)[0];
            var key = "very_popular_item";
            _cache.Set(key, item, TimeSpan.FromSeconds(3));
            Parallel.For(0, 100000, i =>
            {
                _cache.Get<DemoObject>(key, TimeSpan.FromSeconds(0));
            });
            var cachedItem = _cache.Get<DemoObject>(key, TimeSpan.FromSeconds(0));
            Assert.NotNull(cachedItem);
        }

    }
}