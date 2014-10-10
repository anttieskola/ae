using AE.Mpg.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AE.WebUI.Models
{
    public class MpgViewModel
    {
        public int Selected { get; set; }
        public IEnumerable<Vehicle> Cars { get; set; }
    }
}