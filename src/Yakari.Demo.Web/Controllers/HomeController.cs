using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Yakari.Demo.Web.Controllers
{
    public class HomeController : Controller
    {
        ILocalCacheProvider _littleThunder;

        public HomeController (ILocalCacheProvider littleThunder)
        {
          _littleThunder = littleThunder;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
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


