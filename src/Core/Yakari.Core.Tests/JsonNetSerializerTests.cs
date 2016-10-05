using NUnit.Framework;
using Yakari.Core.Serializers.Newtonsoft;

namespace Yakari.Core.Tests
{
    [TestFixture]
    public class JsonNetSerializerTests
    {
        [Test]
        public void TestSerialize()
        {
            var serializer = new JsonNetSerializer();
            var message = new CacheEventMessage("1", "Beaver0", CacheEventType.Get);
            var data = serializer.Serialize(message);
            var deserialized = serializer.Deserialize<CacheEventMessage>(data);
            Assert.AreEqual(message, deserialized);
        }
         
    }
}