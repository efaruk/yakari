using System;
using LightInject;
using Yakari.RedisClient;
using Yakari.Serializers.Newtonsoft;

namespace Yakari.Demo
{
    public class DependencyContainer : IDependencyContainer<ServiceContainer>
    {
        public const string RemoteCacheProviderName = "RemoteCacheProvider";
        public const string ChannelName = "YakariDemo";

        private const string CacheManagerName = "CacheManager";
        private const string ConnectionString = "192.168.99.100:6379,abortConnect=false,defaultDatabase=1,keepAlive=300,resolveDns=false,synctimeout=5000";

        private ServiceContainer _container;
        private ILogger _logger;

        public DependencyContainer(ServiceContainer container, string memberName)
        {
            _container = container ?? new ServiceContainer();
            Setup(_container, memberName);
        }

        private void Setup(ServiceContainer container, string memberName)
        {
            container.SetDefaultLifetime<PerContainerLifetime>();
            //container.Register<ILogger>(factory => new ConsoleLogger(LogLevel.Debug));
            container.Register<ILogger>(factory => new InMemoryLogger(LogLevel.Debug));
            _logger = container.GetInstance<ILogger>();
            _logger.Log("Registering Dependencies...");
            container.Register<IDemoHelper, DemoHelper>();
            container.Register<ISerializer, JsonNetSerializer>();
            container.Register<IRemoteCacheProvider>(factory => new RedisCacheProvider(ConnectionString, factory.GetInstance<ISerializer>(), factory.GetInstance<ILogger>()), RemoteCacheProviderName);
            container.GetInstance<IRemoteCacheProvider>(RemoteCacheProviderName);
            container.Register<ISubscriptionManager>(factory
                => new RedisSubscriptionManager(ConnectionString, ChannelName, factory.GetInstance<ILogger>()));
            container.Register<ILocalCacheProviderOptions>(factory => new LocalCacheProviderOptions(factory.GetInstance<ILogger>()));
            container.Register<ILocalCacheProvider, LittleThunder>();
            if (string.IsNullOrEmpty(memberName)) memberName = Guid.NewGuid().ToString();
            container.Register<ICacheObserver>(factory
                => new GreatEagle(memberName, factory.GetInstance<ISubscriptionManager>(), factory.GetInstance<ISerializer>(), factory.GetInstance<ILocalCacheProvider>(), factory.GetInstance<IRemoteCacheProvider>(RemoteCacheProviderName), factory.GetInstance<ILogger>())
            , CacheManagerName);
            Initialize();
        }

        private void Initialize()
        {
            Resolve<ICacheObserver>();
        }

        public void Replace(ServiceContainer container)
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
