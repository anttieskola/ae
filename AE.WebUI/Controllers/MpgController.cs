using AE.Mpg.Abstract;
using AE.Mpg.Entity;
using AE.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AE.WebUI.Controllers
{
    public class MpgController : Controller
    {
        public string Index()
        {
            return "Placeholder.";
        }

        //private ICarRepository _cars;
        //private IFillRepository _fills;

        //public MpgController(ICarRepository cars, IFillRepository fills)
        //{
        //    if (cars == null)
        //    {
        //        throw new ArgumentNullException("cars must be defined.");
        //    }
        //    if (fills == null)
        //    {
        //        throw new ArgumentNullException("fills must be defined.");
        //    }
        //    _cars = cars;
        //    _fills = fills;
        //}

        //public ActionResult Index()
        //{
        //    MpgViewModel vm = new MpgViewModel { Selected = _cars.Cars.First().CarId, Cars = _cars.Cars };
        //    return View(vm);
        //}

        //public PartialViewResult Fills(int? id)
        //{
        //    if (id == null)
        //    {
        //        return PartialView("_Fills", Enumerable.Empty<Fill>());
        //    }
        //    return PartialView("_Fills", _fills.Fills((int)id));
        //}
    }
}