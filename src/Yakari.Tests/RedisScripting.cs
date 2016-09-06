using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StackExchange.Redis;

namespace Yakari.Tests
{
    [TestFixture]
    public class RedisScripting
    {
        private IDatabase _database;
        private ConnectionMultiplexer _redisConnectionMultiplexer;
        private IServer _redisServer;
        private const int TestItemCount = 10000;

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _redisConnectionMultiplexer = ConnectionMultiplexer.Connect("172.17.0.1:6379,abortConnect=false,defaultDatabase=0,keepAlive=300,resolveDns=false,synctimeout=5000");
            _redisServer = _redisConnectionMultiplexer.GetServer("172.17.0.1:6379");
            _database = _redisConnectionMultiplexer.GetDatabase();
        }

        [SetUp]
        public void Setup()
        {
            var keys = _redisServer.Keys();
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
