using AE.Mpg.Abstract;
using AE.Mpg.Entity;
using AE.WebUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AE.WebUI.Controllers.Api
{
    public class MpgController : ApiController
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

        [HttpGet]
        public VehicleFills Get(int? id)
        {
            if (id == null)
            {
                return new VehicleFills { Success = false, Error = "Id must be defined." };
            }
            VehicleFills res = new VehicleFills { Success = true, Error = ""};
            res.Vehicle = _vehicles.Get((int)id);
            res.Fills = from f in _fills.Get()
                        where f.VehicleId == id
                        select f;
            return res;
        }
    }
}
