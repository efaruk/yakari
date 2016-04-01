using System;
using NSubstitute;
using NUnit.Framework;

namespace Yakari.Tests
{
    [TestFixture]
    public class CacheProviderOptionsTests
    {
        [Test]
        public void When_all_options_are_blank_Then_it_should_throw_argument_null_exception()
        {
            Assert.Throws<ArgumentNullException>(() => { var options = new LocalCacheProviderOptions(null, null); });
        }

        [Test]
        public void When_obserer_is_null_should_throw_argument_null_exception()
        {
            var logger = Substitute.For<ILogger>();
            Assert.Throws<ArgumentNullException>(() => { var options = new LocalCacheProviderOptions(logger, null); });
        }

        [Test]
        public void When_logger_is_null_should_throw_argument_null_exception()
        {
            var observer = Substitute.For<ICacheManager>();
            Assert.Throws<ArgumentNullException>(() => { var options = new LocalCacheProviderOptions(null, observer); });
        }

        [Test]
        public void When_set_maxretry_less_then_1_it_should_throw_argument_outofrange_exception()
        {
            var logger = Substitute.For<ILogger>();
            var observer = Substitute.For<ICacheManager>();
            Assert.Throws<ArgumentOutOfRangeException>(() => { var options = new LocalCacheProviderOptions(logger, observer, 0); });
        }

        [Test]
        public void When_not_set_default_options_concurrencylevel_should_equals_to_10()
        {
            var logger = Substitute.For<ILogger>();
            var observer = Substitute.For<ICacheManager>();
            var options = new LocalCacheProviderOptions(logger, observer);
            Assert.AreEqual(options.ConcurrencyLevel, 10);
        }

        [Test]
        public void When_not_set_default_options_initialcapacity_should_equals_to_100()
        {
            var logger = Substitute.For<ILogger>();
            var observer = Substitute.For<ICacheManager>();
            var options = new LocalCacheProviderOptions(logger, observer);
            Assert.AreEqual(options.InitialCapacity, 100);
        }

        [Test]
        public void When_not_set_default_options_maxretry_should_equals_to_1000()
        {
            var logger = Substitute.For<ILogger>();
            var observer = Substitute.For<ICacheManager>();
            var options = new LocalCacheProviderOptions(logger, observer);
            Assert.AreEqual(options.MaxRetryForLocalOperations, 1000);
        }
    }
}
