using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AE.WebUI.Controllers
{
    public class BlogController : Controller
    {
        [HttpGet]
        public ViewResult New()
        {
            return View();
        }

        [HttpGet]
        public PartialViewResult Upload()
        {
            return PartialView("_Upload");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public string Upload(HttpPostedFileBase file)
        {
            // next we would upload image to a blob storage as a practice
            // windows azure storage emulator http://msdn.microsoft.com/en-us/library/azure/hh403989.aspx
            return "failure.";
        }
    }
}