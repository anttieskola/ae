using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AE.News.Entity
{
    public class Maintenance
    {
        public int MaintenanceId { get; set; }
        public bool Success { get; set; }
        public string Exception { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Inserted { get; set; }
        public int Deleted { get; set; }
    }
}
