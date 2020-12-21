using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Shopf
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "SidebarPartial",
               url: "Page/SidebarPartial",
               defaults: new { controller = "Page", action = "SidebarPartial", id = UrlParameter.Optional },
               new[] { "Shopf.Controllers" }
           );
            /////////
            ///
            routes.MapRoute(
               name: "Shop",
               url: "Shop/{action}/{name}",
               defaults: new { controller = "Shop", action = "Index",name = UrlParameter.Optional, id = UrlParameter.Optional },
               new[] { "Shopf.Controllers" }
           );
            routes.MapRoute(
               name: "Page",
               url: "{page}",
               defaults: new { controller = "Page", action = "Index", id = UrlParameter.Optional },
               new[] { "Shopf.Controllers" }
           );
            routes.MapRoute(
               name: "Default",
               url: "",
               defaults: new { controller = "Page", action = "Index", id = UrlParameter.Optional },
               new[] { "Shopf.Controllers" }
           );
            routes.MapRoute(
              name: "PagesMenuPartial",
              url: "Page/PagesMenuPartial",
              defaults: new { controller = "Page", action = "PagesMenuPartial", id = UrlParameter.Optional },
              new[] { "Shopf.Controllers" }
          );

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}
