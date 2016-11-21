using Yakari;
using Yakari.RedisClient;
using Yakari.Serializer.Newtonsoft;

namespace Yakari.Pack
{
    public class YakariPack
    {
        ILogger _logger;
        ISerializer _serializer;
        IRemoteCacheProvider _remoteCacheProvider;
        ISubscriptionManager _subscriptionManager;
        ILocalCacheProvider _localCacheProvider;
        ICacheObserver _observer;

        public YakariPack(ILocalCacheProviderOptions localCacheProviderOptions, string tribeName, string memberName, string redisConnectionString, LogLevel loglevel)
        {
            _logger = new ConsoleLogger(loglevel);
            // Default Serializer
            _serializer = new JsonNetSerializer();
            //Redis Remote Cache Provider for Yakari
            _remoteCacheProvider = new RedisCacheProvider(redisConnectionString, _serializer, _logger);
            //Redis Subscription Manager for tribe communication.
            _subscriptionManager = new RedisSubscriptionManager(redisConnectionString, tribeName, _logger);
            // Little Thunder the Local Cache Provider
            _localCacheProvider = new LittleThunder(localCacheProviderOptions, _logger);
            // The Great Eagle
            _observer = new GreatEagle(memberName, _subscriptionManager, _serializer, _localCacheProvider, _remoteCacheProvider, _logger);
            // Great eagle start observing and loads every previous remote cache items in seperate thread
        }


        public ILocalCacheProvider LocalCacheProvider
        {
            get
            {
                return _localCacheProvider;
            }
        }
    }
}