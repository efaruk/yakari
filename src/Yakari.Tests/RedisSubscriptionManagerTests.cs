using LightInject;
using NSubstitute;
using NUnit.Framework;
using StackExchange.Redis;
using Yakari.Interfaces;
using Yakari.RedisClient;
using Yakari.Serializer.Newtonsoft;

namespace Yakari.Tests
{
    [TestFixture]
    public class RedisSubscriptionManagerTests
    {
        ILogger _logger;
        ServiceContainer _container;
        IConnectionMultiplexer _mockConnectionMultiplexer;
        ISubscriber _mockSubscriber;
        ISubscriptionManager _subscriptionManager;
        ISerializer _serializer;

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _container = new ServiceContainer();
            _container.SetDefaultLifetime<PerContainerLifetime>();
            _container.Register<ILogger>(factory => new ConsoleLogger(LogLevel.Debug));
            _logger = _container.GetInstance<ILogger>();
            _container.Register<ISerializer, JsonNetSerializer>();
            _serializer = _container.GetInstance<ISerializer>();
            _mockConnectionMultiplexer = Substitute.For<IConnectionMultiplexer>();
            _container.RegisterInstance(typeof(IConnectionMultiplexer), _mockConnectionMultiplexer);
            _mockSubscriber = Substitute.For<ISubscriber>();
            _container.RegisterInstance(typeof(ISubscriber), _mockSubscriber);
            _mockConnectionMultiplexer.GetSubscriber().Returns(_container.GetInstance<ISubscriber>());
            _container.Register<ISubscriptionManager>(factory => new RedisSubscriptionManager(factory.GetInstance<IConnectionMultiplexer>(), TestConstants.ChannelName, factory.GetInstance<ILogger>()));
            _subscriptionManager = _container.GetInstance<ISubscriptionManager>();
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