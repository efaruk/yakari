using System;
using NSubstitute;
using NUnit.Framework;
using Yakari;

namespace Yakari.Tests
{
    [TestFixture]
    public class CacheProviderOptionsTests
    {
        [Test]
        public void When_set_maxretry_less_then_1_it_should_throw_argument_outofrange_exception()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => { var options = new LocalCacheProviderOptions(0); });
        }

        [Test]
        public void When_not_set_default_options_concurrencylevel_should_equals_to_10()
        {
            var options = new LocalCacheProviderOptions();
            Assert.AreEqual(options.ConcurrencyLevel, 10);
        }

        [Test]
        public void When_not_set_default_options_initialcapacity_should_equals_to_100()
        {
            var options = new LocalCacheProviderOptions();
            Assert.AreEqual(options.InitialCapacity, 100);
        }

        [Test]
        public void When_not_set_default_options_maxretry_should_equals_to_1000()
        {
            var options = new LocalCacheProviderOptions();
            Assert.AreEqual(options.MaxRetryForLocalOperations, 1000);
        }
    }
}
