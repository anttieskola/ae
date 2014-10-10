using AE.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace AE.WebUI
{
    public class Global : HttpApplication
    {
        /// <summary>
        /// app "entry" point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // news updater
            if (!NewsDaemon.Instance.Start())
            {
                throw new Exception("NewsUpdate job fails to launch.");
            }
        }

        /// <summary>
        /// cleaning up app resources
        /// </summary>
        public override void Dispose()
        {
            NewsDaemon.Instance.Stop();
            base.Dispose();
        }
    }
}