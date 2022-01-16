using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Yakari;
using Yakari.Demo.Helper;
using Yakari.RedisClient;
using Yakari.Serializer.Newtonsoft;

namespace Yakari.Demo
{
    public class DemoDependencyContainer : IDependencyContainer
    {
        //public const string RemoteCacheProviderName = "RemoteCacheProvider";
        public const string ChannelName = "YakariDemo";
        
        static readonly string RedisIP = string.IsNullOrEmpty(SettingsHelper.Redis) ? "docker-host:6379" : SettingsHelper.Redis;


        readonly string _connectionString = $"{RedisIP},abortConnect=false,defaultDatabase=1,keepAlive=300,resolveDns=false,synctimeout=5000,allowAdmin=true";

        IServiceCollection _services;
        IServiceProvider _serviceProvider;
        //ILogger _logger;

        public DemoDependencyContainer(IServiceCollection services, string memberName = "")
        {
            _services = services;
            if (string.IsNullOrWhiteSpace(memberName))
            {
                memberName = string.Format("Beaver-{0}-{1}", ChannelName, Guid.NewGuid().ToString("N"));
            }
            ConfigureServices(memberName);
        }

        private IServiceCollection ConfigureServices(string memberName)
        {   
            // Default Logger
            //_logger = new InMemoryLogger(LogLevel.Debug);
            //builder.RegisterInstance(_logger).As<ILogger>().SingleInstance();
            //_logger.Log("Registering Dependencies...");
            // Demo Helper
            //builder.Register(c => new DemoHelper(ChannelName, memberName)).As<IDemoHelper>().SingleInstance();
            _services.AddSingleton<IDemoHelper, DemoHelper>(p => new DemoHelper(ChannelName, memberName));
            // Default Serializer
            //builder.RegisterType<JsonNetSerializer>().As<ISerializer>().SingleInstance();
            _services.AddSingleton<ISerializer, JsonNetSerializer>();
            //Redis Remote Cache Provider for Yakari
            //builder.Register(
            //            c =>
            //                new RedisCacheProvider(_connectionString, c.Resolve<ISerializer>(), c.Resolve<ILogger>()))
            //            .As<IRemoteCacheProvider>()
            //            .SingleInstance();
            _services.AddSingleton<IRemoteCacheProvider, RedisCacheProvider>(p => new RedisCacheProvider(_connectionString, p.GetRequiredService<ISerializer>(), p.GetRequiredService<ILogger<RedisCacheProvider>>()));

            //Redis Subscription Manager for tribe communication.
            //builder.Register(c => new RedisSubscriptionManager(_connectionString, ChannelName, c.Resolve<ILogger>())).As<ISubscriptionManager>().SingleInstance();
            _services.AddSingleton<ISubscriptionManager, RedisSubscriptionManager>(p => new RedisSubscriptionManager(_connectionString, ChannelName, p.GetRequiredService<ILogger<RedisSubscriptionManager>>()));
            // Options class for LittleThunder.
            //builder.RegisterType<LocalCacheProviderOptions>().As<ILocalCacheProviderOptions>().SingleInstance();
            _services.AddSingleton<ILocalCacheProviderOptions, LocalCacheProviderOptions>();
            // Little Thunder the Local Cache Provider
            //builder.RegisterType<LittleThunder>().As<ILocalCacheProvider>().SingleInstance();
            _services.AddSingleton<ILocalCacheProvider, LittleThunder>();
            // The Great Eagle
            //builder.Register(
            //            c =>
            //                new GreatEagle(memberName, c.Resolve<ISubscriptionManager>(), c.Resolve<ISerializer>(),
            //                    c.Resolve<ILocalCacheProvider>(), c.Resolve<IRemoteCacheProvider>(),
            //                    c.Resolve<ILogger>())).As<ICacheObserver>().SingleInstance();
            _services.AddSingleton<ICacheObserver, GreatEagle>(p => new GreatEagle(memberName, p.GetRequiredService<ISubscriptionManager>(), p.GetRequiredService<ISerializer>(), 
                p.GetRequiredService<ILocalCacheProvider>(), p.GetRequiredService<IRemoteCacheProvider>(), p.GetRequiredService<ILogger<GreatEagle>>()));

            return _services;
        }

        public void Setup(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Resolve<ICacheObserver>();
        }

        public T Resolve<T>()
        {
            var t = _serviceProvider.GetRequiredService<T>();
            return t;
        }

        public object Resolve(Type type)
        {
            var t = _serviceProvider.GetRequiredService(type);
            return t;
        }
        
    }
}
