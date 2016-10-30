using System;
using Autofac;
using Nancy.Bootstrappers.Autofac;
using Yakari.RedisClient;
using Yakari.Serializer.Newtonsoft;

namespace Yakari.Demo.Api
{
    public class HostAutofacBootstrapper : AutofacNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(ILifetimeScope container)
        {
            // Application name, desired to share same cache items
            var tribeName = "IstanbulCoders";
            // To seperate app instances, diagnostinc purposes, * Must ne unique: You can use Guid.NewGuid().ToString();
            var memberName = Guid.NewGuid().ToString();
            //StackExchange.Redis connectionstring
            var redisConnectionString = "172.17.0.1:6379,abortConnect=false,defaultDatabase=1,keepAlive=300,resolveDns=false,synctimeout=5000,allowAdmin=true";
            // Default Logger
            var logger = new InMemoryLogger(LogLevel.Info);
            container.Update(b => b.RegisterInstance(logger).As<ILogger>().SingleInstance());
            // Default Serializer
            container.Update(b => b.RegisterType<JsonNetSerializer>().As<ISerializer>().SingleInstance());
            //Redis Remote Cache Provider for Yakari
            container.Update(
                b =>
                    b.Register(
                        c =>
                            new RedisCacheProvider(redisConnectionString, c.Resolve<ISerializer>(), c.Resolve<ILogger>()))
                        .As<IRemoteCacheProvider>()
                        .SingleInstance());
            //Redis Subscription Manager for tribe communication.
            container.Update(b => b.Register(c => new RedisSubscriptionManager(redisConnectionString, tribeName, c.Resolve<ILogger>())).As<ISubscriptionManager>().SingleInstance());
            // Options class for LittleThunder.
            container.Update(
                b => b.RegisterType<LocalCacheProviderOptions>().As<ILocalCacheProviderOptions>().SingleInstance());
            // Little Thunder the Local Cache Provider
            container.Update(b => b.RegisterType<LittleThunder>().As<ILocalCacheProvider>().SingleInstance());
            // The Great Eagle
            container.Update(
                b =>
                    b.Register(
                        c =>
                            new GreatEagle(memberName, c.Resolve<ISubscriptionManager>(), c.Resolve<ISerializer>(),
                                c.Resolve<ILocalCacheProvider>(), c.Resolve<IRemoteCacheProvider>(),
                                c.Resolve<ILogger>())).As<ICacheObserver>().SingleInstance());
            // Great eagle start observing and loads every previous remote cache items in seperate thread
            container.Resolve<ICacheObserver>();
        }

    }
}