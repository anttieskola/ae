using AE.WebUI.Filters;
using System.Web;
using System.Web.Mvc;

namespace AE.WebUI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            // filters.Add(new MinificationFilter());
        }
    }
}
