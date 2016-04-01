using System;
using LightInject;
using NSubstitute;
using NUnit.Framework;
using Yakari.Demo;
using Yakari.RedisClient;
using Yakari.Serializers.Newtonsoft;

namespace Yakari.Tests
{
    [TestFixture]
    public class GreatEagleTests
    {
        public const string LocalCacheProviderName = "LocalCacheProvider";
        public const string RemoteCacheProviderName = "RemoteCacheProvider";
        public const string ChannelName = "YakariDemo";
        private const string CacheManagerName = "CacheManager";

        private ILogger _logger;
        private ServiceContainer _container;


        [OneTimeSetUp]
        public void Setup()
        {
            _container = new ServiceContainer();
            _container.SetDefaultLifetime<PerContainerLifetime>();
            _container.Register<ILogger>(factory => new ConsoleLogger(LogLevel.Debug));
            _logger = _container.GetInstance<ILogger>();
            _logger.Log("Registering Dependencies...");
            _container.Register<IDemoHelper, DemoHelper>();
            _container.Register<ISerializer<string>, JsonSerializer>();
            //container.Register<ICacheProvider>((factory) => new RedisCacheProvider("192.168.99.100:6379,abortConnect=false,defaultDatabase=1,keepAlive=300,resolveDns=false,syncTimeout=10000", factory.GetInstance<ISerializer<string>>(), factory.GetInstance<ILogger>()), RemoteCacheProviderName);
            var redisCacheProvider = Substitute.For<ICacheProvider>();
            //container.Register<ISubscriptionManager>(factory => new RedisSubscriptionManager("192.168.99.100:6379,abortConnect=false,defaultDatabase=1,keepAlive=300,resolveDns=false,syncTimeout=10000", ChannelName));
            var subscriptionManager = Substitute.For<ISubscriptionManager>();
            _container.Register<IMessagePublisher>(factory => subscriptionManager);
            _container.Register<IMessageSubscriber>(factory => subscriptionManager);
            _container.Register<ICacheManager>(factory
                => new GreatEagle(Guid.NewGuid().ToString(), factory.GetInstance<IMessagePublisher>(), factory.GetInstance<IMessageSubscriber>(),
                    factory.GetInstance<ISerializer<string>>(), redisCacheProvider, factory.GetInstance<ILogger>())
                    , CacheManagerName);
            var manager = _container.GetInstance<ICacheManager>(CacheManagerName);
            //container.Register<ILocalCacheProviderOptions>(factory => new LocalCacheProviderOptions(factory.GetInstance<ILogger>(), factory.GetInstance<ICacheManager>()));
            //container.Register<ICacheProvider, LittleThunder>(LocalCacheProviderName);
            var localProvider = Substitute.For<ICacheProvider>();
            manager.SetupMember(localProvider);
        }

        [Test]
        public void SetupTest()
        {
            var manager = _container.GetInstance<ICacheManager>();
            var helper = _container.GetInstance<IDemoHelper>();
            var list = helper.GenerateDemoObjects(1000);
            var key = Guid.NewGuid().ToString();
            var item = new InMemoryCacheItem(list, TimeSpan.FromMinutes(15));
            manager.OnBeforeGet(key, TimeSpan.FromSeconds(3));
            manager.OnAfterGet(key);
            manager.OnAfterDelete(key);
            manager.OnBeforeSet(key, item);
            manager.OnAfterSet(key, item);
            manager.OnBeforeDelete(key);
            manager.OnRemoteSet(key, CacheManagerName);
            manager.OnRemoteDelete(key, CacheManagerName);
        }
    }
}