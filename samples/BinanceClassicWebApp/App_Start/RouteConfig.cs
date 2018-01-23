using System.Web.Mvc;
using System.Web.Routing;
// ReSharper disable ArgumentsStyleOther
// ReSharper disable ArgumentsStyleStringLiteral

namespace BinanceClassicWebApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
