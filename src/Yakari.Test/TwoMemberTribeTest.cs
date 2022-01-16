﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Yakari.Demo;

namespace Yakari.Tests
{
    [TestFixture]
    public class TwoMemberTribeTest
    {
        DemoDependencyContainer _member1Container;
        DemoDependencyContainer _member2Container;
        IDemoHelper _demoHelper;

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            _member1Container = new DemoDependencyContainer(null, "Member 1");
            _member2Container = new DemoDependencyContainer(null, "Member 2");
            _demoHelper = _member1Container.Resolve<IDemoHelper>();
        }

        [Test]
        public void Mermber1SetMember2ExistsTest()
        {
            var key = "demo_object_list";
            var items = _demoHelper.GenerateDemoObjects(1000);
            var member1Local = _member1Container.Resolve<ILocalCacheProvider>();
            var member2Local = _member2Container.Resolve<ILocalCacheProvider>();
            member1Local.Set(key, items, CacheTime.FifteenMinutes, false);
            var list1 = member1Local.Get<List<DemoObject>>(key, TimeSpan.FromSeconds(5), false);
            var list2 = member2Local.Get<List<DemoObject>>(key, TimeSpan.FromSeconds(5), false);
            Assert.AreEqual(list1.Count, list2.Count);
        }


    }
}
