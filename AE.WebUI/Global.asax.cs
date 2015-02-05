using AE.Insomnia;
using System;
using System.Configuration;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AE.WebUI
{
    public class Global : HttpApplication
    {
        private void runMigrations()
        {
            AE.Users.Migrations.Run.Migration();
            AE.Mpg.Migrations.Run.Migration();
        }

        /// <summary>
        /// app "entry" point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Start(object sender, EventArgs e)
        {
            if (bool.Parse(ConfigurationManager.AppSettings["MigrateDatabaseToLatestVersion"]))
            {
                runMigrations();
            }
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            InsomniaDaemon.Instance.Start();
        }
    }
}