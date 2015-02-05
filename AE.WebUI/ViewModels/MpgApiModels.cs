using AE.Mpg.Entity;
using System.Collections.Generic;

namespace AE.WebUI.ViewModels
{
    public class VehicleFills
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public Vehicle Vehicle { get; set; }
        public IEnumerable<Fill> Fills { get; set; }
    }
}
