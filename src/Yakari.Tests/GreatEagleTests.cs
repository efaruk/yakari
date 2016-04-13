using System;
using LightInject;
using NSubstitute;
using NUnit.Framework;
using Yakari.Demo;
using Yakari.Serializers.Newtonsoft;

namespace Yakari.Tests
{
    [TestFixture]
    public class GreatEagleTests
    {
        private ILogger _logger;
        private ServiceContainer _container;
        private ICacheManager _manager;


        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _container = new ServiceContainer();
            _container.SetDefaultLifetime<PerContainerLifetime>();
            _container.Register<ILogger>(factory => new ConsoleLogger(LogLevel.Debug));
            _logger = _container.GetInstance<ILogger>();
            _logger.Log("Registering Dependencies...");
            _container.Register<IDemoHelper, DemoHelper>();
            _container.Register<ISerializer<string>, JsonNetSerializer>();
            //container.Register<ICacheProvider>((factory) => new RedisCacheProvider("192.168.99.100:6379,abortConnect=false,defaultDatabase=1,keepAlive=300,resolveDns=false,syncTimeout=10000", factory.GetInstance<ISerializer<string>>(), factory.GetInstance<ILogger>()), RemoteCacheProviderName);
            var redisCacheProvider = Substitute.For<IRemoteCacheProvider>();
            //container.Register<ISubscriptionManager>(factory => new RedisSubscriptionManager("192.168.99.100:6379,abortConnect=false,defaultDatabase=1,keepAlive=300,resolveDns=false,syncTimeout=10000", ChannelName));
            var subscriptionManager = Substitute.For<ISubscriptionManager>();
            _container.Register<IMessagePublisher>(factory => subscriptionManager);
            _container.Register<IMessageSubscriber>(factory => subscriptionManager);
            var localCacheProvider = Substitute.For<ILocalCacheProvider>();
            _container.Register(factory => localCacheProvider);
            _container.Register<ICacheManager>(factory
                => new GreatEagle(Guid.NewGuid().ToString(), factory.GetInstance<IMessagePublisher>(), factory.GetInstance<IMessageSubscriber>(),
                    factory.GetInstance<ISerializer<string>>(), localCacheProvider, redisCacheProvider, factory.GetInstance<ILogger>())
                    , TestConstants.CacheManagerName);
            _manager = _container.GetInstance<ICacheManager>(TestConstants.CacheManagerName);
            //container.Register<ILocalCacheProviderOptions>(factory => new LocalCacheProviderOptions(factory.GetInstance<ILogger>(), factory.GetInstance<ICacheManager>()));
            //container.Register<ICacheProvider, LittleThunder>(LocalCacheProviderName);
        }

        [Test]
        public void SetupTest()
        {
            var helper = _container.GetInstance<IDemoHelper>();
            var list = helper.GenerateDemoObjects(1000);
            var key = Guid.NewGuid().ToString();
            var item = new InMemoryCacheItem(list, TimeSpan.FromMinutes(15));
            _manager.OnBeforeGet(key, TimeSpan.FromSeconds(3));
            _manager.OnAfterGet(key);
            _manager.OnAfterDelete(key);
            _manager.OnBeforeSet(key, item);
            _manager.OnAfterSet(key, item);
            _manager.OnBeforeDelete(key);
            _manager.OnRemoteSet(key, TestConstants.CacheManagerName);
            _manager.OnRemoteDelete(key, TestConstants.CacheManagerName);
        }
    }
}