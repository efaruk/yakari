using System;
using System.Collections.Generic;
using NUnit.Framework;
using Yakari.Demo;
using Yakari.Pack;

namespace Yakari.Tests
{
    public class YakariPackTest
    {
        YakariPack _pack1;
        YakariPack _pack2;
        IDemoHelper _demoHelper;
       


        

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            var _connectionString = "127.0.0.1:6379,abortConnect=false,defaultDatabase=1,keepAlive=300,resolveDns=false,synctimeout=5000,allowAdmin=true";

            _pack1 = new YakariPack(new LocalCacheProviderOptions(), "myTribe", "beaver1", _connectionString, LogLevel.Info);
            _pack2 = new YakariPack(new LocalCacheProviderOptions(), "myTribe", "beaver1", _connectionString, LogLevel.Info);
            _demoHelper = new DemoHelper();
        }

        [Test]
        public void Mermber1SetMember2ExistsTest()
        {
            var key = "demo_object_list";
            var items = _demoHelper.GenerateDemoObjects(1000);
            var member1Local = _pack1.LocalCacheProvider;
            var member2Local = _pack2.LocalCacheProvider;
            member1Local.Set(key, items, CacheTime.FifteenMinutes, false);
            var list1 = member1Local.Get<List<DemoObject>>(key, TimeSpan.FromSeconds(5), false);
            var list2 = member2Local.Get<List<DemoObject>>(key, TimeSpan.FromSeconds(5), false);
            Assert.AreEqual(list1.Count, list2.Count);
        }


    }
}
