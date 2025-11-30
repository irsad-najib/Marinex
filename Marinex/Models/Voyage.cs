using System;

namespace Marinex.Models
{
    public class Voyage
    {
        public int VoyageID { get; set; }
        public string From { get; set; }
        public string Destination { get; set; }
        public TimeSpan EstimatedDuration { get; set; }

        // Relasi
        public int ShipID { get; set; }
        public int UserID { get; set; }
    }
}
