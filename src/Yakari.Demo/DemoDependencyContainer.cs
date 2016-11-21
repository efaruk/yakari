using System;
using Autofac;
using Yakari;
using Yakari.Demo.Helper;
using Yakari.RedisClient;
using Yakari.Serializer.Newtonsoft;

namespace Yakari.Demo
{
    public class DemoDependencyContainer : IDependencyContainer<IContainer>
    {
        //public const string RemoteCacheProviderName = "RemoteCacheProvider";
        public const string ChannelName = "YakariDemo";

        const string CacheManagerName = "CacheManager";
        static readonly string RedisIP = string.IsNullOrEmpty(SettingsHelper.Redis) ? "127.0.0.1:6379" : SettingsHelper.Redis;


        readonly string _connectionString = $"{RedisIP},abortConnect=false,defaultDatabase=1,keepAlive=300,resolveDns=false,synctimeout=5000,allowAdmin=true";

        IContainer _container;
        ILogger _logger;

        public DemoDependencyContainer(IContainer container, string memberName)
        {
            _container = container;
            if (_container == null) {
             _container = Setup(memberName);   
            }
        }

        IContainer Setup(string memberName)
        {
            var builder = new ContainerBuilder();
            
            // Default Logger
            _logger = new InMemoryLogger(LogLevel.Debug);
            builder.RegisterInstance(_logger).As<ILogger>().SingleInstance();
            _logger.Log("Registering Dependencies...");
            // Demo Helper
            builder.Register(c => new DemoHelper(ChannelName, memberName)).As<IDemoHelper>().SingleInstance();
            // Default Serializer
            builder.RegisterType<JsonNetSerializer>().As<ISerializer>().SingleInstance();
            //Redis Remote Cache Provider for Yakari
            builder.Register(
                        c =>
                            new RedisCacheProvider(_connectionString, c.Resolve<ISerializer>(), c.Resolve<ILogger>()))
                        .As<IRemoteCacheProvider>()
                        .SingleInstance();
            //Redis Subscription Manager for tribe communication.
            builder.Register(c => new RedisSubscriptionManager(_connectionString, ChannelName, c.Resolve<ILogger>())).As<ISubscriptionManager>().SingleInstance();
            // Options class for LittleThunder.
            builder.RegisterType<LocalCacheProviderOptions>().As<ILocalCacheProviderOptions>().SingleInstance();
            // Little Thunder the Local Cache Provider
            builder.RegisterType<LittleThunder>().As<ILocalCacheProvider>().SingleInstance();
            // The Great Eagle
            builder.Register(
                        c =>
                            new GreatEagle(memberName, c.Resolve<ISubscriptionManager>(), c.Resolve<ISerializer>(),
                                c.Resolve<ILocalCacheProvider>(), c.Resolve<IRemoteCacheProvider>(),
                                c.Resolve<ILogger>())).As<ICacheObserver>().SingleInstance();
            _container = builder.Build();
            Initialize();
            return _container;
        }

        void Initialize()
        {
            Resolve<ICacheObserver>();
        }

        public void Replace(IContainer container)
        {
            _container = container;
        }

        public T Resolve<T>()
        {
            var t = _container.Resolve<T>();
            return t;
        }

        public T Resolve<T>(string name)
        {
            var t = _container.ResolveNamed<T>(name);
            return t;
        }

        public object Resolve(Type type)
        {
            var t = _container.Resolve(type);
            return t;
        }        

        public IDisposable BeginScope()
        {
            return _container.BeginLifetimeScope();
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}
