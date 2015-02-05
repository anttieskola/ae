using AE.Mpg.Abstract;
using AE.Mpg.Entity;
using AE.WebUI.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AE.WebUI.Controllers.View
{
    [Authorize]
    public class MpgAdminController : Controller
    {
        private IGenericRepository<Vehicle> _vehicles;
        private IGenericRepository<Fill> _fills;
        private IGenericRepository<Fuel> _fuels;

        /// <summary>
        /// injection constructor
        /// </summary>
        /// <param name="vehicles"></param>
        /// <param name="fills"></param>
        public MpgAdminController(
            IGenericRepository<Vehicle> vehicles,
            IGenericRepository<Fill> fills,
            IGenericRepository<Fuel> fuels)
        {
            _vehicles = vehicles;
            _fills = fills;
            _fuels = fuels;
        }

        #region Vehicles
        public ActionResult Vehicles()
        {
            return View(_vehicles.Get());
        }

        public ActionResult VehicleCreate()
        {
            var vm = new VehicleCreateViewModel
            {
                Vehicle = new Vehicle(),
                Fuels = new SelectList(_fuels.Get(), "FuelId", "Name")
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VehicleCreate(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                await _vehicles.Insert(vehicle);
                return RedirectToAction("Vehicles");
            }
            ModelState.AddModelError("", "Data not valid.");
            return View(vehicle);
        }

        public ActionResult VehicleEdit(int id)
        {
            var vm = new VehicleEditViewModel
            {
                Vehicle = _vehicles.Get(id),
                Fuels = new SelectList(_fuels.Get(), "FuelId", "Name")
            };
            if (vm.Vehicle != null)
            {
                return View(vm);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VehicleEdit(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                await _vehicles.Update(vehicle);
                return RedirectToAction("Vehicles");
            }
            ModelState.AddModelError("", "Data not valid.");
            return View(vehicle);
        }

        [HttpGet]
        public ActionResult VehicleDelete(int? id)
        {
            if (id != null)
            {
                Vehicle v = _vehicles.Get((int)id);
                if (v != null)
                {
                    return View(v);
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VehicleDelete(int id)
        {
            _vehicles.Delete(id);
            return RedirectToAction("Vehicles");
        }
        #endregion

        #region Fillups
        public ActionResult Fills(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var vm = new VehicleFillsViewModel();
            vm.Vehicle = _vehicles.Get((int)id);
            vm.Fills = from f in _fills.Get()
                       where f.VehicleId == id
                       select f;
            return View(vm);
        }

        public ActionResult FillCreate(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (_vehicles.Get((int)id) == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(new Fill { VehicleId = (int)id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FillCreate(Fill fill)
        {
            if (ModelState.IsValid)
            {
                await _fills.Insert(fill);
                return RedirectToAction("Index", "Mpg");
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FillUpdate(Fill fill)
        {
            await _fills.Update(fill);
            return RedirectToAction("Fills", new { id = fill.VehicleId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FillDelete(Fill fill)
        {
            await _fills.Delete(fill.FillId);
            return RedirectToAction("Fills", new { id = fill.VehicleId });
        }
        #endregion
    }
}