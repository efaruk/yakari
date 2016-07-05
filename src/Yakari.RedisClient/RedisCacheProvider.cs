using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace Yakari.RedisClient
{
    public class RedisCacheProvider: BaseCacheProvider, IRemoteCacheProvider
    {
        private readonly ISerializer _serializer;
        private readonly ILogger _logger;
        private ConnectionMultiplexer _redisConnectionMultiplexer;
        private IDatabase _database;

        public RedisCacheProvider(string connectionString, ISerializer serializer, ILogger logger)
        {
            _serializer = serializer;
            _logger = logger;
            SetupConfiguration(connectionString);
        }

        public RedisCacheProvider(ConfigurationOptions redisConfigurationOptions, ISerializer serializer, ILogger logger)
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
            //TODO: it can be true, but we should work on it (Another feature)
            get { return false; }
        }

        public override T Get<T>(string key, TimeSpan getTimeout, bool isManagerCall)
        {
            _logger.Log(LogLevel.Trace, string.Format("RedisCacheProvider Get {0}", key));
            var data = (string)_database.StringGet(key, CommandFlags.PreferSlave);
            // Make get with sliding
            var item = _serializer.Deserialize<T>(data);
            return item;
        }

        public override void Set(string key, object value, TimeSpan expiresIn, bool isManagerCall)
        {
            _logger.Log(LogLevel.Debug, string.Format("RedisCacheProvider Set {0}", key));
            var data = _serializer.Serialize(value);
            _database.StringSet(key, data.ToString(), expiresIn, When.Always, CommandFlags.DemandMaster);
        }

        public override void Delete(string key, bool isManagerCall)
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

        public override List<string> AllKeys()
        {
            _logger.Log(LogLevel.Trace, "RedisCacheProvider AllKeys");
            var script = "local result = redis.call(\'SCAN\', \'0\', \'MATCH\', \'*\', \'COUNT\', \'1000000\');\r\nreturn result[2];";
            var lua = LuaScript.Prepare(script);
            var result = _database.ScriptEvaluate(lua);
            var keys = new string [] { };
            if (!result.IsNull)
            {
                keys = (string[])result;
            }
            var list = keys.ToList();
            return list;
        }
    }
}