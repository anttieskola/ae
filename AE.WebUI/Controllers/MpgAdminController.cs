using AE.Mpg.Abstract;
using AE.Mpg.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AE.WebUI.Controllers
{
    [Authorize(Roles="Admin")]
    public class MpgAdminController : Controller
    {
        private IGenericRepository<Vehicle> _vehicles;
        private IGenericRepository<Fill> _fills;

        /// <summary>
        /// injection constructor
        /// </summary>
        /// <param name="vehicles"></param>
        /// <param name="fills"></param>
        public MpgAdminController(
            IGenericRepository<Vehicle> vehicles,
            IGenericRepository<Fill> fills)
        {
            _vehicles = vehicles;
            _fills = fills;
        }

        [HttpGet]
        public ViewResult Vehicles()
        {
            return View(_vehicles.Get(orderBy: q => q.OrderBy(r => r.VehicleId)));
        }

        [HttpGet]
        public ViewResult Fills(int? carId)
        {
            if (carId == null)
            {
                return View(_fills.Get(orderBy: q => q.OrderBy(r => r.FillId)));
            }
            return View(_fills.Get(f => f.VehicleId == carId));
        }

        [HttpPost]
        public bool UpdateVehicle(Vehicle v)
        {
            return false;
        }

        [HttpPost]
        public bool UpdateFill(Fill f)
        {
            return false;
        }

        [HttpPost]
        public bool DeleteVehicle(int id)
        {
            return false;
        }

        [HttpPost]
        public bool DeleteFill(int id)
        {
            return false;
        }
    }
}