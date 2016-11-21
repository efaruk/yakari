using System;
using Autofac;
using NSubstitute;
using NUnit.Framework;
using Yakari.Demo;
using Yakari;
using Yakari.Serializer.Newtonsoft;

namespace Yakari.Tests
{
    [TestFixture]
    public class GreatEagleTests
    {
        ILogger _logger;
        IContainer _container;
        ICacheObserver _observer;


        [OneTimeSetUp]
        public void FixtureSetup()
        {
            var memberName = "DefaultMember";
            var builder = new ContainerBuilder();
            _logger = new ConsoleLogger(LogLevel.Debug);
            builder.RegisterInstance(_logger).As<ILogger>();
            _logger.Log("Registering Dependencies...");
            builder.RegisterType<DemoHelper>().As<IDemoHelper>().SingleInstance();
            builder.RegisterType<JsonNetSerializer>().As<ISerializer>().SingleInstance();
            var remoteCacheProvider = Substitute.For<IRemoteCacheProvider>();
            var subscriptionManager = Substitute.For<ISubscriptionManager>();
            builder.RegisterInstance(remoteCacheProvider).As<IRemoteCacheProvider>().SingleInstance();
            builder.RegisterInstance(subscriptionManager).As<ISubscriptionManager>().SingleInstance();
            var localCacheProvider = Substitute.For<ILocalCacheProvider>();
            builder.RegisterInstance(localCacheProvider).As<ILocalCacheProvider>().SingleInstance();
            builder.Register(
                        c =>
                            new GreatEagle(memberName, c.Resolve<ISubscriptionManager>(), c.Resolve<ISerializer>(),
                                c.Resolve<ILocalCacheProvider>(), c.Resolve<IRemoteCacheProvider>(),
                                c.Resolve<ILogger>())).As<ICacheObserver>().SingleInstance();
            _container = builder.Build();

            _observer = _container.Resolve<ICacheObserver>();
        }

        [Test]
        public void SetupTest()
        {
            var helper = _container.Resolve<IDemoHelper>();
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