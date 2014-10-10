using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AE.WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "ThisIsJustANameCouldBeAnything",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "News" ,action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

