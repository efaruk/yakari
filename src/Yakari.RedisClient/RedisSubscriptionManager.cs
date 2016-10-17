using System;
using StackExchange.Redis;
using Yakari;

namespace Yakari.RedisClient
{
    public class RedisSubscriptionManager : ISubscriptionManager
    {
        string _channelName;
        readonly ILogger _logger;
        static IConnectionMultiplexer _redisConnectionMultiplexer;

        public RedisSubscriptionManager(string connectionString, string channelName, ILogger logger)
        {
            _logger = logger;
            _logger.Log(LogLevel.Trace, $"RedisSubscriptionManager : connectionString={connectionString}, channelName={channelName}");
            _channelName = channelName;
            _redisConnectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
        }

        public RedisSubscriptionManager(ConfigurationOptions redisConfigurationOptions, string channelName, ILogger logger)
        {
            _logger = logger;
            _logger.Log(LogLevel.Trace, $"RedisSubscriptionManager : redisConfigurationOptions={redisConfigurationOptions}, channelName={channelName}");
            _channelName = channelName;
            _redisConnectionMultiplexer = ConnectionMultiplexer.Connect(redisConfigurationOptions);
        }

        public RedisSubscriptionManager(IConnectionMultiplexer redisConnectionMultiplexer, string channelName, ILogger logger)
        {
            _logger = logger;
            _channelName = channelName;
            _redisConnectionMultiplexer = redisConnectionMultiplexer;
        }

        public void ReConnect(string connectionString, string channelName,  bool waitForDisconnect = true)
        {
            _logger.Log(LogLevel.Trace, $"RedisSubscriptionManager ReConnect: connectionString={connectionString}, channelName={channelName}");
            _channelName = channelName;
            _redisConnectionMultiplexer?.Close(waitForDisconnect);
            _redisConnectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
        }

        public void ReConnect(ConfigurationOptions redisConfigurationOptions, string channelName, bool waitForDisconnect = true)
        {
            _logger.Log(LogLevel.Trace, $"RedisSubscriptionManager ReConnect: redisConfigurationOptions={redisConfigurationOptions}, channelName={channelName}");
            _channelName = channelName;
            _redisConnectionMultiplexer?.Close(waitForDisconnect);
            _redisConnectionMultiplexer = ConnectionMultiplexer.Connect(redisConfigurationOptions);
        }

        ISubscriber Subscriber => _redisConnectionMultiplexer.GetSubscriber();

        public virtual void Publish(string message)
        {
            _logger.Log(LogLevel.Trace, $"RedisSubscriptionManager Publish Message : {message}");
            if (Subscriber == null) throw InvalidOperationException();
            var redisChannel = new RedisChannel(_channelName, RedisChannel.PatternMode.Auto);
            Subscriber.PublishAsync(redisChannel, message);
        }

        void OnReceived(RedisChannel redisChannel, RedisValue redisValue)
        {
            if (_channelName != redisChannel) return;
            MessageReceived(redisValue);
        }

        public void StartSubscription()
        {
            _logger.Log(LogLevel.Trace, "RedisSubscriptionManager StartSubscription");
            if (Subscriber == null) throw InvalidOperationException();
            Subscriber.Subscribe(_channelName, OnReceived);
        }

        public void StopSubscription()
        {
            _logger.Log(LogLevel.Trace, "RedisSubscriptionManager StopSubscription");
            if (Subscriber == null) throw InvalidOperationException();
            Subscriber.Unsubscribe(_channelName, OnReceived);
        }

        public void MessageReceived(string message)
        {
            _logger.Log(LogLevel.Trace, $"RedisSubscriptionManager Message Received: {message}");
            OnMessageReceived?.Invoke(message);
        }

        public event MessageReceived OnMessageReceived;

        InvalidOperationException InvalidOperationException() { return new InvalidOperationException("Subscriber can't be null, manager not initialized properly..."); }
    }
}
