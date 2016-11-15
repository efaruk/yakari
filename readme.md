# Yakari

![Yakari Logo](https://github.com/efaruk/yakari/blob/master/docs/logo/YakariLogoSmall.png)

[![Build status](https://ci.appveyor.com/api/projects/status/0e86yl55qxo81xkr?svg=true)](https://ci.appveyor.com/project/efaruk/yakari)

Yakari is simply in memory cache distributor.

Project is aimed to reduce distributed caching systems serialization/deserialization and network operation costs.
You simply work with your localCache, the GreatEagle observe's your operations and take care of everything.
In yakari if you set 1 object it makes 1 serialization and 1 network operation, for first get operation it makes 1 network operation, 1 deserialization, after then there is no deserialization or network operation you can get your cache object from your localCache, how much you want. 

## Yakari is Very Easy
---

1. Install: Using Nuget
    ```powershell
    Install-Package Yakari.Pack
    ```

2. Configure:

    ```csharp
    // Application name, desired to share same cache items
    var tribeName = "MyTribe";
    // To seperate app instances, diagnostinc purposes, * Must ne unique: You can use Guid.NewGuid().ToString();
    var memberName = "Beaver1";
    //StackExchange.Redis connectionstring
    var redisConnectionString = "127.0.0.1:6379,abortConnect=false,defaultDatabase=1,keepAlive=300,resolveDns=false,synctimeout=5000,allowAdmin=true";
    // Default Logger
    var logger = new ConsoleLogger(LogLevel.Info);
    // Default Serializer
    var serializer = new JsonNetSerializer();
    //Redis Remote Cache Provider for Yakari
    var remoteCacheProvider = new RedisCacheProvider(redisConnectionString, serializer, logger);
    //Redis Subscription Manager for tribe communication.
    var subscriptionManager = new RedisSubscriptionManager(redisConnectionString, tribeName, logger);
    // Options class for LittleThunder.
    var localCacheProviderOptions = new LocalCacheProviderOptions(logger);
    // Little Thunder the Local Cache Provider
    var localCacheProvider = new LittleThunder(localCacheProviderOptions);
    // The Great Eagle
    var observer = new GreatEagle(memberName, subscriptionManager, serializer, localCacheProvider, remoteCacheProvider, logger);
    // Great eagle start observing and loads every previous remote cache items in seperate thread
    ```

3. Use:

    ```csharp
    // Usage
    var key = "pebbles";

    // Simple Set
    localCacheProvider.Set(key, new[] { "pebble1", "pebble2", "pebble3" }, CacheTime.FifteenMinutes);

    // Simple Get
    var pebbles = localCacheProvider.Get<string[]>(key, TimeSpan.FromSeconds(5));

    // Get with Acquire Function *Recommended
    var item = localCacheProvider.Get<string[]>(key, TimeSpan.FromSeconds(5), () =>
        {
            return new[] { "pebble1", "pebble2", "pebble3" };
        }, CacheTime.FifteenMinutes);

    // Simple Delete
    localCacheProvider.Delete(key);
    ```

Happy distributing :smile:


All tests are passing, we are stable now, please feed us...


We Need Help for
---

1. Documentation
2. Test
3. Review
4. Coding
5. Porting Other Languages, [Java:Jakari](https://github.com/TitaniumSoft/jakari), [Go:Gakari](https://github.com/TitaniumSoft/gakari), etc...

You can ping me at: https://twitter.com/farukpehlivanli or https://tr.linkedin.com/in/efaruk

Our #Slack Team Domain: https://yakariteam.slack.com/


Please [![Tweet](http://i.imgur.com/wWzX9uB.png)](https://twitter.com/intent/tweet?url=https://github.com/efaruk/yakari&text=Yakari%20Cache%20Distributor&hashtags=Distributed,InMemory,Cache,dotnet) us ;)


# Many Thanks to Our Sponsors: 

[![kloia](https://www.google.com/a/cpanel/kloia.com/images/logo.gif)](http://kloia.com)

<a href="https://www.jetbrains.com/" title="JetBrains Rule'z" target="_blank"><img src="https://resources.jetbrains.com/assets/media/open-graph/jetbrains_250x250.png" height="55"></img></a>

- to be a [sponsor](https://tr.linkedin.com/in/efaruk)
