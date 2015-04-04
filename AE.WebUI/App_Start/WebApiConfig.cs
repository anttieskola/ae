using System.Web.Http;

namespace AE.WebUI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableSystemDiagnosticsTracing();
            config.MapHttpAttributeRoutes();
            // /api/news/articles
            config.Routes.MapHttpRoute(
                name: "ControllerActionParameter",
                routeTemplate: "api/{controller}/{action}");
            // /api/mpg/id
            config.Routes.MapHttpRoute(
                name: "ControllerParameter",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }
    }
}
