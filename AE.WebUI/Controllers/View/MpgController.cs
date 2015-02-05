using AE.Mpg.Abstract;
using AE.Mpg.Entity;
using AE.WebUI.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace AE.WebUI.Controllers.View
{
    public class MpgController : Controller
    {
        private IGenericRepository<Vehicle> _vehicles;
        private IGenericRepository<Fill> _fills;

        public MpgController(IGenericRepository<Vehicle> vehicles, IGenericRepository<Fill> fills)
        {
            if (vehicles == null)
            {
                throw new ArgumentNullException("vehicles must be defined.");
            }
            if (fills == null)
            {
                throw new ArgumentNullException("fills must be defined.");
            }
            _vehicles = vehicles;
            _fills = fills;
        }

        public ActionResult Index()
        {
            MpgViewModel vm = new MpgViewModel();
            vm.Vehicles = from v in _vehicles.Get()
                          select v;
            if (vm.Vehicles.Count() > 0)
            {
                vm.Selected = vm.Vehicles.First().VehicleId;
            }
            return View(vm);
        }
    }
}