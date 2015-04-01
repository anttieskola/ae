using System;

namespace AE.Funny.Entity
{
    public class Maintenance
    {
        public int MaintenanceId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Inserted { get; set; }
        public int Deleted { get; set; }
    }
}
