using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightInject;
using NSubstitute;
using NUnit.Framework;
using Yakari.Demo;

namespace Yakari.Tests
{
    public class TwoMemberTribeTest
    {
        private DependencyContainer member1Container;
        private DependencyContainer member2Container;
        private IDemoHelper _demoHelper;

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            member1Container = new DependencyContainer(null, "Member 1");
            member2Container = new DependencyContainer(null, "Member 2");
            _demoHelper = member1Container.Resolve<IDemoHelper>();
        }

        [Test]
        public void Mermber1SetMember2ExistsTest()
        {
            var key = "demo_object_list";
            var items = _demoHelper.GenerateDemoObjects(1000);
            var member1Local = member1Container.Resolve<ILocalCacheProvider>();
            var member2Local = member2Container.Resolve<ILocalCacheProvider>();
            member1Local.Set(key, items, CacheTime.FifteenMinutes);
            items = member1Local.Get<List<DemoObject>>(key, TimeSpan.Zero);
            var list = member2Local.Get<List<DemoObject>>(key, TimeSpan.Zero);
            Assert.AreEqual(items, list);
        }


    }
}
