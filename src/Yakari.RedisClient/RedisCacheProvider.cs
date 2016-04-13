using System;
using StackExchange.Redis;

namespace Yakari.RedisClient
{
    public class RedisCacheProvider: BaseCacheProvider, IRemoteCacheProvider
    {
        private readonly ISerializer<string> _serializer;
        private readonly ILogger _logger;
        private ConnectionMultiplexer _redisConnectionMultiplexer;
        private IDatabase _database;

        public RedisCacheProvider(string connectionString, ISerializer<string> serializer, ILogger logger)
        {
            _serializer = serializer;
            _logger = logger;
            SetupConfiguration(connectionString);
        }

        public RedisCacheProvider(ConfigurationOptions redisConfigurationOptions, ISerializer<string> serializer, ILogger logger)
        {
            _serializer = serializer;
            _logger = logger;
            SetupConfiguration(redisConfigurationOptions);
        }

        public void ReConnect(string connectionString, bool waitForDisconnect = true)
        {
            if (_redisConnectionMultiplexer != null)
            {
                _redisConnectionMultiplexer.Close(waitForDisconnect);
            }
            SetupConfiguration(connectionString);
        }

        public void ReConnect(ConfigurationOptions redisConfigurationOptions, bool waitForDisconnect = true)
        {
            if (_redisConnectionMultiplexer != null)
            {
                _redisConnectionMultiplexer.Close(waitForDisconnect);
            }
            SetupConfiguration(redisConfigurationOptions);
        }

        private void SetupConfiguration(string connectionString)
        {
            _redisConnectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
            _database = _redisConnectionMultiplexer.GetDatabase();
        }

        private void SetupConfiguration(ConfigurationOptions redisConfigurationOptions)
        {
            _redisConnectionMultiplexer = ConnectionMultiplexer.Connect(redisConfigurationOptions);
            _database = _redisConnectionMultiplexer.GetDatabase();
        }

        public override void Dispose()
        {
            _logger.Log(LogLevel.Trace, "RedisCacheProvider Disposing");
            _redisConnectionMultiplexer.Dispose();
        }

        public override bool HasSlidingSupport
        {
            get { return true; }
        }

        public override T Get<T>(string key, TimeSpan getTimeout)
        {
            _logger.Log(LogLevel.Trace, string.Format("RedisCacheProvider Get {0}", key));
            var data = (string)_database.StringGet(key, CommandFlags.PreferSlave);
            // Make get with sliding
            var item = _serializer.Deserialize<T>(data);
            return item;
        }

        public override void Set(string key, object value, TimeSpan expiresIn)
        {
            _logger.Log(LogLevel.Debug, string.Format("RedisCacheProvider Set {0}", key));
            var data = _serializer.Serialize(value);
            _database.StringSet(key, data, expiresIn, When.Always, CommandFlags.DemandMaster);
        }

        public override void Delete(string key)
        {
            _logger.Log(LogLevel.Trace, string.Format("RedisCacheProvider Delete {0}", key));
            _database.KeyDelete(key, CommandFlags.DemandMaster);
        }

        public override bool Exists(string key)
        {
            _logger.Log(LogLevel.Trace, string.Format("RedisCacheProvider Exists {0}", key));
            var exists = _database.KeyExists(key);
            return exists;
        }
    }
}