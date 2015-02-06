using System;

namespace AE.Mpg.Entity
{
    public class Fill
    {
        public int FillId { get; set; }
        public float Mileage { get; set; }
        public float Amount { get; set; }
        public DateTime Date { get; set; }
        public float? Price { get; set; }
        public int VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }
    }
}
