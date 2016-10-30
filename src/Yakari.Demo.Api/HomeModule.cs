using Nancy;

namespace Yakari.Demo.Api
{
    public class HomeModule: NancyModule
    {
        public HomeModule(ILocalCacheProvider localCacheProvider)
        {
            Get["/"] = p => Response.AsText("Hellow world from Nancyfx");

            Get["/cache/"] = p =>
            {
                return Response.AsJson("Summary");
            };
        }
    }
}
