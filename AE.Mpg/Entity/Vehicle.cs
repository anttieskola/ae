using AE.Users.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AE.Mpg.Entity
{
    public class Vehicle
    {
        public int VehicleId { get; set; }

        public string Make { get; set; }

        public int ManufacturingYear { get; set; }

        public string Model { get; set; }

        public string Engine { get; set; }

        public int FuelId { get; set; }

        public virtual Fuel Fuel { get; set; }

        public bool Private { get; set; }

        public string AspNetUserId { get; set; }

        public virtual AspNetUser User { get; set; }
    }
}
