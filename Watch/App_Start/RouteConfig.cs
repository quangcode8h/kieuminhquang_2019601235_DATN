using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Watch
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "list product catefory",
                url: "danh-muc/{Metatitle}-{ID}",
                defaults: new { controller = "Home", action = "lstProBy_Category", id = UrlParameter.Optional },
                namespaces: new[] { "Watch.Controllers" }
            );
            routes.MapRoute(
                name: "list product brand",
                url: "thuong-hieu/{Metatitle}-{ID}",
                defaults: new { controller = "Home", action = "lstProBy_Brand", id = UrlParameter.Optional },
                namespaces: new[] { "Watch.Controllers" }
            );
            routes.MapRoute(
                name: "search product",
                url: "tim-kiem",
                defaults: new { controller = "Product", action = "Search", id = UrlParameter.Optional },
                namespaces: new[] { "Watch.Controllers" }
            );
             routes.MapRoute(
                name: "product detail",
                url: "san-pham/{Metatitle}-{ID}",
                defaults: new { controller = "Product", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Watch.Controllers" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Watch.Controllers" }
            );
        }
    }
}
