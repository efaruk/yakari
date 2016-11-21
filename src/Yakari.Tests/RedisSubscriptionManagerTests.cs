using Autofac;
using NSubstitute;
using NUnit.Framework;
using StackExchange.Redis;
using Yakari;
using Yakari.RedisClient;
using Yakari.Serializer.Newtonsoft;

namespace Yakari.Tests
{
    [TestFixture]
    public class RedisSubscriptionManagerTests
    {
        ILogger _logger;
        IContainer _container;
        IConnectionMultiplexer _mockConnectionMultiplexer;
        ISubscriber _mockSubscriber;
        ISubscriptionManager _subscriptionManager;
        ISerializer _serializer;

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            var builder = new ContainerBuilder();
            _logger = new ConsoleLogger(LogLevel.Debug);
            builder.RegisterInstance(_logger).As<ILogger>().SingleInstance();
            builder.RegisterType<JsonNetSerializer>().As<ISerializer>().SingleInstance();
            _mockConnectionMultiplexer = Substitute.For<IConnectionMultiplexer>();
            builder.RegisterInstance(_mockConnectionMultiplexer).As<IConnectionMultiplexer>().SingleInstance();
            _mockSubscriber = Substitute.For<ISubscriber>();
            builder.RegisterInstance(_mockSubscriber).As<ISubscriber>().SingleInstance();
            builder.Register(c => new RedisSubscriptionManager(c.Resolve<IConnectionMultiplexer>(), TestConstants.ChannelName, c.Resolve<ILogger>())).As<ISubscriptionManager>().SingleInstance();
            
            _container = builder.Build();
            _subscriptionManager = _container.Resolve<ISubscriptionManager>();
            _mockConnectionMultiplexer.GetSubscriber().Returns(_container.Resolve<ISubscriber>());
            _serializer = _container.Resolve<ISerializer>();
            _logger.Log("RedisSubscriptionManagerTests SetupComplete");
        }

        [Test]
        public void TestPublishSimpleMessage()
        {
            var message = "Hellow world again!";
            _subscriptionManager.Publish(message);
            _mockSubscriber.Received().PublishAsync(TestConstants.ChannelName, message);
        }

        [Test]
        public void TestCacheEventMessagePublish()
        {
            var message = _serializer.Serialize(new CacheEventMessage("000-111-999-000", "Beaver1", CacheEventType.Delete));
            _subscriptionManager.Publish(message.ToString());
            _mockSubscriber.Received().PublishAsync(TestConstants.ChannelName, message.ToString());
        }

    }
}