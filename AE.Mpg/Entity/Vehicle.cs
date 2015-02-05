using AE.Users.Entity;
using System.ComponentModel.DataAnnotations;

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
    }
}
