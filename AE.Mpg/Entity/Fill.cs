using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AE.Mpg.Entity
{
    public class Fill
    {
        public int FillId { get; set; }

        public float Mileage { get; set; }

        public float Amount { get; set; }

        public int VehicleId { get; set; }

        public virtual Vehicle Vehicle { get; set; }
    }
}
