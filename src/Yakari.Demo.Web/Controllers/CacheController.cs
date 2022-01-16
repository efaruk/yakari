namespace Yakari.Demo.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    public class CacheController : Controller
    {
        private readonly ILocalCacheProvider _littleThunder;
        private readonly IDemoHelper _demoHelper;
        private const string KeyFormat = "{0}:{1}";

        public CacheController(ILocalCacheProvider littleThunder, IDemoHelper demoHelper)
        {
            _littleThunder = littleThunder;
            _demoHelper = demoHelper;
        }

        // GET api/cache
        [HttpGet]
        public List<string> Get()
        {
            var keys = _littleThunder.AllKeys();
            return keys;
        }

        // GET api/cache
        [HttpGet("generate")]
        public object Generate()
        {
            var list = _demoHelper.GenerateDemoObjects(1000);
            var key = string.Format(KeyFormat, _demoHelper.MemberName, Guid.NewGuid());
            _littleThunder.Set(key, list, CacheTime.OneDay, false);
            return list;
        }

        // GET api/cache/5
        [HttpGet("{id}")]
        public object Get(string id)
        {
            var item = _littleThunder.Get<List<DemoObject>>(id, TimeSpan.FromSeconds(3), false);
            return item;
        }
        
        // POST api/cache
        [HttpPost]
        public void Post()
        {
            var list = _demoHelper.GenerateDemoObjects(1000);
            var key = string.Format(KeyFormat, _demoHelper.MemberName, Guid.NewGuid());
            _littleThunder.Set(key, list, CacheTime.OneDay, false);
        }

        // PUT api/cache/5
        [HttpPut("{id}")]
        public void Put(string id)
        {
            var list = _demoHelper.GenerateDemoObjects(1000);
            _littleThunder.Set(id, list, CacheTime.OneDay, false);
        }

        // DELETE api/cache/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _littleThunder.Delete(id, false);
        }
    }
}
