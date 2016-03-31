using System;
using StackExchange.Redis;

namespace Yakari.RedisClient
{
    public class RedisSubscriptionManager : IMessagePublisher, IMessageSubscriber
    {
        private string _channelName;
        private static ConnectionMultiplexer _redisConnectionMultiplexer;

        public RedisSubscriptionManager(string connectionString, string channelName)
        {
            _channelName = channelName;
            _redisConnectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
        }

        public RedisSubscriptionManager(ConfigurationOptions redisConfigurationOptions, string channelName)
        {
            _channelName = channelName;
            _redisConnectionMultiplexer = ConnectionMultiplexer.Connect(redisConfigurationOptions);
        }

        public void ReConnect(string connectionString, string channelName,  bool waitForDisconnect = true)
        {
            _channelName = channelName;
            if (_redisConnectionMultiplexer != null)
            {
                _redisConnectionMultiplexer.Close(waitForDisconnect);
            }
            _redisConnectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
        }

        public void ReConnect(ConfigurationOptions redisConfigurationOptions, string channelName, bool waitForDisconnect = true)
        {
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
            if (Subscriber == null) throw InitalizationException();
            var redisChannel = new RedisChannel(_channelName, RedisChannel.PatternMode.Auto);
            Subscriber.Publish(redisChannel, message);
        }

        private void OnReceived(RedisChannel redisChannel, RedisValue redisValue)
        {
            if (_channelName != redisChannel) return;
            MessageReceived(redisValue);
        }

        public void StartSubscription()
        {
            if (Subscriber == null) throw InitalizationException();
            Subscriber.Subscribe(_channelName, OnReceived);
        }

        public void StopSubscription()
        {
            if (Subscriber == null) throw InitalizationException();
            Subscriber.Unsubscribe(_channelName, OnReceived);
        }

        public void MessageReceived(string message)
        {
            if (OnMessageReceived != null)
            {
                OnMessageReceived(message);
            }
        }

        public event MessageReceived OnMessageReceived;

        private InvalidOperationException InitalizationException() { return new InvalidOperationException("Subscriber can't be null, manager not initialized properly..."); }
    }
}
