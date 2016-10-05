Yakari
======

![Yakari Logo](https://github.com/efaruk/yakari/blob/master/docs/logo/YakariLogoSmall.png)

[![Build status](https://ci.appveyor.com/api/projects/status/0e86yl55qxo81xkr?svg=true)](https://ci.appveyor.com/project/efaruk/yakari)

Yakari is simply in memory cache distributor.

Project is aimed to reduce distributed caching systems serialization/deserialization and network operation costs.
You simply work with your localCache, the GreatEagle observe's your operations and take care of everything.
In yakari if you set 1 object it makes 1 serialization and 1 network operation, for first get operation it makes 1 network operation, 1 deserialization, after then there is no deserialization or network operation you can get your cache object from your localCache, how much you want. 

And it will be easy as adding Yakari nuget package **(coming soon)**...

Under heavy development, don't try anything fancy, just wait for developments(Press **Watch** if you really curious...)

Currently unit tests are passing but there are lot more to do...

We Need Help for
---

1. Documentation
2. Test
3. Review
4. Coding

You can ping me at: https://twitter.com/farukpehlivanli or https://tr.linkedin.com/in/efaruk

Our #Slack Team Domain: https://yakariteam.slack.com/


Please [![Tweet](http://i.imgur.com/wWzX9uB.png)](https://twitter.com/intent/tweet?url=https://github.com/efaruk/yakari&text=Yakari%20Cache%20Distributor&hashtags=Distributed,InMemory,Cache,dotnet) us ;)

Thanks and regards...
