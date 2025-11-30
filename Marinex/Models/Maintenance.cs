using System;

namespace Marinex.Models
{
    public class Maintenance
    {
        public int MaintenanceID { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }

        public int ShipID { get; set; }

    }
}
