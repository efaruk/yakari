using System;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Yakari.RedisClient;
using Yakari.Serializer.Newtonsoft;

namespace Yakari.Demo.Api
{
    public class HostNancyBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            // Application name, desired to share same cache items
            var tribeName = "IstanbulCoders";
            // To seperate app instances, diagnostinc purposes, * Must ne unique: You can use Guid.NewGuid().ToString();
            var memberName = Guid.NewGuid().ToString();
            //StackExchange.Redis connectionstring
            var redisConnectionString = "172.17.0.1:6379,abortConnect=false,defaultDatabase=1,keepAlive=300,resolveDns=false,synctimeout=5000,allowAdmin=true";
            // Default Logger
            var logger = new InMemoryLogger(LogLevel.Info);
            container.Register<ILogger>((c, p) => new InMemoryLogger(LogLevel.Info));
            // Default Serializer
            container.Register<ISerializer, JsonNetSerializer>().AsSingleton();
            //Redis Remote Cache Provider for Yakari
            container.Register<IRemoteCacheProvider>((c, p) => new RedisCacheProvider(redisConnectionString, c.Resolve<ISerializer>(), c.Resolve<ILogger>()));
            //Redis Subscription Manager for tribe communication.
            container.Register<ISubscriptionManager>(
                (c, p) => new RedisSubscriptionManager(redisConnectionString, tribeName, c.Resolve<ILogger>()));
            // Options class for LittleThunder.
            container.Register<ILocalCacheProviderOptions>((c, p) => new LocalCacheProviderOptions(c.Resolve<ILogger>()));
            // Little Thunder the Local Cache Provider
            container.Register<ILocalCacheProvider, LittleThunder>().AsSingleton();
            // The Great Eagle
            container.Register<ICacheObserver>(
                (c, p) =>
                    new GreatEagle(memberName, c.Resolve<ISubscriptionManager>(), c.Resolve<ISerializer>(),
                        c.Resolve<ILocalCacheProvider>(), c.Resolve<IRemoteCacheProvider>(), c.Resolve<ILogger>()));
            // Great eagle start observing and loads every previous remote cache items in seperate thread
            container.Resolve<ICacheObserver>();
        }
    }
}