using System;
using StackExchange.Redis;

namespace Yakari.RedisClient
{
    public class RedisSubscriptionManager : ISubscriptionManager
    {
        private string _channelName;
        private readonly ILogger _logger;
        private static IConnectionMultiplexer _redisConnectionMultiplexer;

        public RedisSubscriptionManager(string connectionString, string channelName, ILogger logger)
        {
            _logger = logger;
            _logger.Log(LogLevel.Trace, string.Format("RedisSubscriptionManager : connectionString={0}, channelName={1}", connectionString, channelName));
            _channelName = channelName;
            _redisConnectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
        }

        public RedisSubscriptionManager(ConfigurationOptions redisConfigurationOptions, string channelName, ILogger logger)
        {
            _logger = logger;
            _logger.Log(LogLevel.Trace, string.Format("RedisSubscriptionManager : redisConfigurationOptions={0}, channelName={1}", redisConfigurationOptions, channelName));
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
            _logger.Log(LogLevel.Trace, string.Format("RedisSubscriptionManager ReConnect: connectionString={0}, channelName={1}", connectionString, channelName));
            _channelName = channelName;
            if (_redisConnectionMultiplexer != null)
            {
                _redisConnectionMultiplexer.Close(waitForDisconnect);
            }
            _redisConnectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
        }

        public void ReConnect(ConfigurationOptions redisConfigurationOptions, string channelName, bool waitForDisconnect = true)
        {
            _logger.Log(LogLevel.Trace, string.Format("RedisSubscriptionManager ReConnect: redisConfigurationOptions={0}, channelName={1}", redisConfigurationOptions, channelName));
            _channelName = channelName;
            if (_redisConnectionMultiplexer != null)
            {
                _redisConnectionMultiplexer.Close(waitForDisconnect);
            }
            _redisConnectionMultiplexer = ConnectionMultiplexer.Connect(redisConfigurationOptions);
        }

        private ISubscriber Subscriber
        {
            get { return _redisConnectionMultiplexer.GetSubscriber(); }
        }

        public virtual void Publish(string message)
        {
            _logger.Log(LogLevel.Trace, string.Format("RedisSubscriptionManager Publish Message : {0}", message));
            if (Subscriber == null) throw InvalidOperationException();
            var redisChannel = new RedisChannel(_channelName, RedisChannel.PatternMode.Auto);
            Subscriber.PublishAsync(redisChannel, message);
        }

        private void OnReceived(RedisChannel redisChannel, RedisValue redisValue)
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
            _logger.Log(LogLevel.Trace, string.Format("RedisSubscriptionManager Message Received: {0}", message));
            if (OnMessageReceived != null)
            {
                OnMessageReceived(message);
            }
        }

        public event MessageReceived OnMessageReceived;

        private InvalidOperationException InvalidOperationException() { return new InvalidOperationException("Subscriber can't be null, manager not initialized properly..."); }
    }
}
