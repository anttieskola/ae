using AE.Users.Entity;
using System;
using System.ComponentModel.DataAnnotations;

namespace AE.Mpg.Entity
{
    public enum UnitOfDistance
    {
        Miles = 1,
        Kilometers = 2
    }

    public class Vehicle
    {
        public int VehicleId { get; set; }
        public string Make { get; set; }
        public int ManufacturingYear { get; set; }
        public string Model { get; set; }
        public string Engine { get; set; }
        public UnitOfDistance UnitOfDistance { get; set; }
        public int FuelId { get; set; }
        public virtual Fuel Fuel { get; set; }
    }
}
