using System.Linq;
using System.Web.Mvc;
using Microsoft.Practices.Unity.Mvc;
using System.Web.Http;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(AE.WebUI.App_Start.UnityActivator), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(AE.WebUI.App_Start.UnityActivator), "Shutdown")]

namespace AE.WebUI.App_Start
{
    /// <summary>
    /// Provides the bootstrapping for integrating Unity with ASP.NET MVC.
    /// </summary>
    public static class UnityActivator
    {
        /// <summary>
        /// Integrates Unity when the application starts.
        /// </summary>
        public static void Start() 
        {
            var container = UnityConfig.GetConfiguredContainer(); // configuration
            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));
            DependencyResolver.SetResolver(new UnityDependencyResolver(container)); // Set as MVC resolver
            GlobalConfiguration.Configuration.DependencyResolver = new UnityResolver(container); // Set as Web API resolver
        }

        /// <summary>
        /// Disposes the Unity container when the application is shut down.
        /// </summary>
        public static void Shutdown()
        {
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }
    }
}