using System.Collections.Generic;
using System.Web;
using System.Web.Optimization;

namespace AE.WebUI
{
    /// <summary>
    /// Using this to get correct order for scripts to load.
    /// </summary>
    public class AsIsBundleOrdered : IBundleOrderer
    {
        public virtual IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }

    /// <summary>
    /// Bundles defined here
    /// </summary>
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // javascript
            var jsBundle = new Bundle("~/bundles/js");
            jsBundle.Orderer = new AsIsBundleOrdered();
            jsBundle
                .Include("~/Scripts/jquery-2.1.1.js")
                .Include("~/Scripts/bootstrap.js");
            bundles.Add(jsBundle);

            // css
            var cssBundle = new Bundle("~/bundles/css");
            cssBundle.Orderer = new AsIsBundleOrdered();
            cssBundle
                .Include("~/Content/bootstrap.css")
                .Include("~/Content/bootstrap-theme.css")
                .Include("~/Content/spinner.css")
                .Include("~/Content/site.css");
            bundles.Add(cssBundle);

            // set optimization on
            BundleTable.EnableOptimizations = true;
        }
    }
}
