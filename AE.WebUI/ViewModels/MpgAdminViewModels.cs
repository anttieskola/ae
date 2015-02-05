using AE.Mpg.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AE.WebUI.ViewModels
{
    public class VehicleCreateViewModel
    {
        public Vehicle Vehicle { get; set; }
        public SelectList Fuels { get; set; }
    }

    public class VehicleEditViewModel
    {
        public Vehicle Vehicle { get; set; }
        public SelectList Fuels { get; set; }
    }

    public class VehicleFillNewViewModel
    {
        public int VehicleId { get; set; }
        public string VehicleName { get; set; }
    }

    public class VehicleFillsViewModel
    {
        public Vehicle Vehicle { get; set; }
        public IEnumerable<Fill> Fills { get; set; }
    }
}