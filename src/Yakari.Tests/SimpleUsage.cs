using System;
using Yakari;
using Yakari.Serializer.Newtonsoft;
using Yakari.RedisClient;

namespace Yakari.Tests
{
    public class SimpleUsage
    {
        public void ConfigureAndUse()
        {
            // Every class should be single instance for AppDomain
            // Application name, desired to share same cache items
            var tribeName = "MyTribe";
            // To seperate app instances, diagnostinc purposes, * Must ne unique: You can use Guid.NewGuid().ToString();
            var memberName = "Beaver1";
            //StackExchange.Redis connectionstring
            var redisConnectionString = "127.0.0.1:6379,abortConnect=false,defaultDatabase=1,keepAlive=300,resolveDns=false,synctimeout=5000,allowAdmin=true";
            // Default Logger
            var logger = new ConsoleLogger(LogLevel.Info);
            // Default Serializer
            var serializer = new JsonNetSerializer();
            //Redis Remote Cache Provider for Yakari
            var remoteCacheProvider = new RedisCacheProvider(redisConnectionString, serializer, logger);
            //Redis Subscription Manager for tribe communication.
            var subscriptionManager = new RedisSubscriptionManager(redisConnectionString, tribeName, logger);
            // Options class for LittleThunder.
            var localCacheProviderOptions = new LocalCacheProviderOptions(logger);
            // Little Thunder the Local Cache Provider
            var localCacheProvider = new LittleThunder(localCacheProviderOptions);
            // The Great Eagle
            var observer = new GreatEagle(memberName, subscriptionManager, serializer, localCacheProvider, remoteCacheProvider, logger);
            // Great eagle start observing and loads every previous remote cache items in seperate thread

            // Usage
            var key = "pebbles";

            // Simple Set
            localCacheProvider.Set(key, new[] { "pebble1", "pebble2", "pebble3" }, CacheTime.FifteenMinutes);

            // Simple Get
            var pebbles = localCacheProvider.Get<string[]>(key, TimeSpan.FromSeconds(5));

            // Get with Acquire Function *Recommended
            var item = localCacheProvider.Get<string[]>(key, TimeSpan.FromSeconds(5), () =>
            {
                return new[] { "pebble1", "pebble2", "pebble3" };
            }, CacheTime.FifteenMinutes);

            // Simple Delete
            localCacheProvider.Delete(key);
        }
    }
}
