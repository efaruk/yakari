using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Yakari.Demo.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILocalCacheProvider _littleThunder;
        private readonly IDemoHelper _demoHelper;

        public HomeController (ILocalCacheProvider littleThunder, IDemoHelper demoHelper)
        {
            _littleThunder = littleThunder;
            _demoHelper = demoHelper;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            ViewData["TribeName"] = _demoHelper.TribeName;
            ViewData["MemberName"] = _demoHelper.MemberName;

            var list = _demoHelper.GenerateDemoObjects(1000);
            var key = string.Format("{0}-{1}", _demoHelper.MemberName, Guid.NewGuid().ToString());
            _littleThunder.Set(key, list, CacheTime.FifteenMinutes);

            var keys = _littleThunder.AllKeys();
            
            ViewData["Data"] = keys;

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}


