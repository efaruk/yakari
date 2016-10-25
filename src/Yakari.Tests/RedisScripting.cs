using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StackExchange.Redis;
using Yakari.Tests.Helper;

namespace Yakari.Tests
{
    [TestFixture]
    public class RedisScripting
    {
        IDatabase _database;
        ConnectionMultiplexer _redisConnectionMultiplexer;
        IServer _redisServer;
        const int TestItemCount = 10000;

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            var redis = SettingsHelper.Redis;
            if (string.IsNullOrEmpty(redis))
                redis = "127.0.0.1:6379";

            _redisConnectionMultiplexer = ConnectionMultiplexer.Connect($"{redis},abortConnect=false,defaultDatabase=0,keepAlive=300,resolveDns=false,synctimeout=5000,allowAdmin=true");
            _redisServer = _redisConnectionMultiplexer.GetServer(redis);
            _database = _redisConnectionMultiplexer.GetDatabase();
        }

        [SetUp]
        public void Setup()
        {
            _redisServer.ConfigSet("save", "");
            var keys = _redisServer.Keys(pageSize:1000000);
            _database.KeyDelete(keys.ToArray());
            for (int i = 0; i < TestItemCount; i++)
            {
                _database.StringSet(i.ToString(), i.ToString());
            }
        }
        
        [TearDown]
        public void TearDown()
        {
            var keys = _redisServer.Keys();
            _database.KeyDelete(keys.ToArray());
        }

        [Test]
        public void LuaTest()
        {
            // Add New Items
            for (int i = 0; i < TestItemCount; i++)
            {
                _database.StringSet(i.ToString(), i.ToString());
            }
            // SCAN 0 MATCH '*' COUNT 1000000
            var script = "local result = redis.call(\'SCAN\', \'0\', \'MATCH\', \'*\', \'COUNT\', \'1000000\');\r\nreturn result[2];";
            var lua = LuaScript.Prepare(script);
            var result = _database.ScriptEvaluate(lua);
            var keys = new RedisKey[] { };
            if (!result.IsNull)
            {
                keys = (RedisKey[])result;
            }
            Assert.AreEqual(TestItemCount, keys.Length);
        }

        [Test]
        public void AllKeys()
        {
            var script = "local result = redis.call(\'SCAN\', \'0\', \'MATCH\', \'*\', \'COUNT\', \'1000000\');\r\nreturn result[2];";
            var lua = LuaScript.Prepare(script);
            var result = _database.ScriptEvaluate(lua);
            var keys = new string[] { };
            if (!result.IsNull)
            {
                keys = (string[])result;
            }
            var list = keys.ToList();
            Assert.AreEqual(TestItemCount, list.Count);
        }

    }
}
