using System.Web.Http;

namespace AE.WebUI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableSystemDiagnosticsTracing();
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "Param",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }
    }
}
