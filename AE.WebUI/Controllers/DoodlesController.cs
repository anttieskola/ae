using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AE.WebUI.Controllers
{
    public class DoodlesController : Controller
    {
        // GET: Doodles
        public ActionResult Index()
        {
            return View();
        }

        // GET: Show given doodle
        public ActionResult Show(string name)
        {
            if (name == null)
            {
                return HttpNotFound();
            }
            return View(name);
        }
    }
}