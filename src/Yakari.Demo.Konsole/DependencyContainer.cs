using System;
using LightInject;
using Yakari.RedisClient;
using Yakari.Serializers.Newtonsoft;

namespace Yakari.Demo.Konsole
{
    public class DependencyContainer : IDependencyContainer<ServiceContainer>
    {
        public const string LocalCacheProviderName = "LocalCacheProvider";
        public const string RemoteCacheProviderName = "RemoteCacheProvider";
        public const string ChannelName = "YakariDemo";
        private const string CacheManagerName = "CacheManager";

        private ServiceContainer _container;
        private ILogger _logger;

        public DependencyContainer(ServiceContainer container)
        {
            _container = container ?? new ServiceContainer();
            Setup(_container);
        }

        private void Setup(ServiceContainer container)
        {
            container.SetDefaultLifetime<PerContainerLifetime>();
            container.Register<ILogger>(factory => new ConsoleLogger(LogLevel.Debug));
            _logger = container.GetInstance<ILogger>();
            _logger.Log("Registering Dependencies...");
            container.Register<IDemoHelper, DemoHelper>();
            container.Register<ISerializer<string>, JsonSerializer>();
            container.Register<ICacheProvider>((factory) => new RedisCacheProvider("192.168.99.100:6379,abortConnect=false,defaultDatabase=1,keepAlive=300,resolveDns=false,syncTimeout=10000", factory.GetInstance<ISerializer<string>>(), factory.GetInstance<ILogger>()), RemoteCacheProviderName);
            var redisCacheProvider = container.GetInstance<ICacheProvider>(RemoteCacheProviderName);
            container.Register<ISubscriptionManager>(factory 
                => new RedisSubscriptionManager("192.168.99.100:6379,abortConnect=false,defaultDatabase=1,keepAlive=300,resolveDns=false,syncTimeout=10000", ChannelName));
            container.Register<IMessagePublisher>(factory => factory.GetInstance<ISubscriptionManager>());
            container.Register<IMessageSubscriber>(factory => factory.GetInstance<ISubscriptionManager>());
            container.Register<ICacheManager>(factory 
                => new GreatEagle(Guid.NewGuid().ToString(), factory.GetInstance<IMessagePublisher>(), factory.GetInstance<IMessageSubscriber>(),
                    factory.GetInstance<ISerializer<string>>(), factory.GetInstance<ICacheProvider>(RemoteCacheProviderName), factory.GetInstance<ILogger>())
            , CacheManagerName);
            var manager = container.GetInstance<ICacheManager>(CacheManagerName);
            container.Register<ILocalCacheProviderOptions>(factory => new LocalCacheProviderOptions(factory.GetInstance<ILogger>(), factory.GetInstance<ICacheManager>()));
            container.Register<ICacheProvider, LittleThunder>(LocalCacheProviderName);
            var localProvider = container.GetInstance<ICacheProvider>(LocalCacheProviderName);
            manager.SetupMember(localProvider);
        }

        public void Reset(ServiceContainer container)
        {
            _container = container;
        }

        public T Resolve<T>()
        {
            var t = _container.GetInstance<T>();
            return t;
        }

        public T Resolve<T>(string name)
        {
            var t = _container.GetInstance<T>(name);
            return t;
        }

        public object Resolve(Type type)
        {
            var t = _container.GetInstance(type);
            return t;
        }

        public object Resolve(Type type, string name)
        {
            var t = _container.GetInstance(type, name);
            return t;
        }

        public IDisposable BeginScope()
        {
            return _container.BeginScope();
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}
