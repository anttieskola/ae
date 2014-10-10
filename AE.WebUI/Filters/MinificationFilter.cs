using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AE.WebUI.Filters
{
    public class MinificationFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext c)
        {
            var res = c.HttpContext.Response;
            res.Filter = new MinificationStream(HttpContext.Current.Response.Filter, HttpContext.Current.Response.ContentEncoding);
        }

        public void OnResultExecuting(ResultExecutingContext c)
        {
            // noop
        }
    }
}