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


