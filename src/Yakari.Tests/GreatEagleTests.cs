using System;
using LightInject;
using NSubstitute;
using NUnit.Framework;
using Yakari.Demo;
using Yakari.Interfaces;
using Yakari.Serializer.Newtonsoft;

namespace Yakari.Tests
{
    [TestFixture]
    public class GreatEagleTests
    {
        ILogger _logger;
        ServiceContainer _container;
        ICacheObserver _observer;


        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _container = new ServiceContainer();
            _container.SetDefaultLifetime<PerContainerLifetime>();
            _container.Register<ILogger>(factory => new ConsoleLogger(LogLevel.Debug));
            _logger = _container.GetInstance<ILogger>();
            _logger.Log("Registering Dependencies...");
            _container.Register<IDemoHelper, DemoHelper>();
            _container.Register<ISerializer, JsonNetSerializer>();
            var remoteCacheProvider = Substitute.For<IRemoteCacheProvider>();
            var subscriptionManager = Substitute.For<ISubscriptionManager>();
            _container.Register(factory => subscriptionManager);
            var localCacheProvider = Substitute.For<ILocalCacheProvider>();
            _container.Register(factory => localCacheProvider);
            _container.Register<ICacheObserver>(factory
                => new GreatEagle(Guid.NewGuid().ToString(), factory.GetInstance<ISubscriptionManager>(), 
                    factory.GetInstance<ISerializer>(), localCacheProvider, remoteCacheProvider, factory.GetInstance<ILogger>())
                    , TestConstants.CacheObserverName);
            _observer = _container.GetInstance<ICacheObserver>(TestConstants.CacheObserverName);
            //container.Register<ILocalCacheProviderOptions>(factory => new LocalCacheProviderOptions(factory.GetInstance<ILogger>(), factory.GetInstance<ICacheObserver>()));
            //container.Register<ICacheProvider, LittleThunder>(LocalCacheProviderName);
        }

        [Test]
        public void SetupTest()
        {
            var helper = _container.GetInstance<IDemoHelper>();
            var list = helper.GenerateDemoObjects(1000);
            var key = Guid.NewGuid().ToString();
            var item = new InMemoryCacheItem(list, TimeSpan.FromMinutes(15));
            _observer.OnBeforeGet(key, TimeSpan.FromSeconds(3));
            _observer.OnAfterGet(key);
            _observer.OnAfterDelete(key);
            _observer.OnBeforeSet(key, item);
            _observer.OnAfterSet(key);
            _observer.OnBeforeDelete(key);
            _observer.OnRemoteSet(key, TestConstants.CacheObserverName);
            _observer.OnRemoteDelete(key, TestConstants.CacheObserverName);
        }
    }
}