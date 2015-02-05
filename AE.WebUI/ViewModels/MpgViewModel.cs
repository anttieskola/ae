using AE.Mpg.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AE.WebUI.ViewModels
{
    public class MpgViewModel
    {
        public int? Selected { get; set; }
        public IEnumerable<Vehicle> Vehicles { get; set; }
    }
}