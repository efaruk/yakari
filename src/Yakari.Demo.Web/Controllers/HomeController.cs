namespace Yakari.Demo.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public class HomeController : Controller
    {
        private readonly ILocalCacheProvider _littleThunder;
        private readonly IDemoHelper _demoHelper;
        private readonly ILogger<HomeController> _logger;

        public HomeController (
            ILogger<HomeController> logger
            , ILocalCacheProvider littleThunder
            , IDemoHelper demoHelper
            )
        {
            _logger = logger;
            _littleThunder = littleThunder;
            _demoHelper = demoHelper;
        }
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            ViewData["TribeName"] = _demoHelper.TribeName;
            ViewData["MemberName"] = _demoHelper.MemberName;
            
            var keys = _littleThunder.AllKeys();
            
            ViewData["Data"] = keys;

            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}


