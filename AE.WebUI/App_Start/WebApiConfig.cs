﻿using System.Web.Http;

namespace AE.WebUI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableSystemDiagnosticsTracing();
            config.MapHttpAttributeRoutes();
            // /api/mycontroller/method
            // /api/mycontroller/method/{id}
            config.Routes.MapHttpRoute(
                name: "ControllerActionParameter",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }
    }
}
