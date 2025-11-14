using System;

namespace Marinex.Models
{
    public class ShipPosition
    {
        public string Mmsi { get; set; }
        public string ShipName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
        public double Course { get; set; }
        public DateTime LastUpdate { get; set; }
        public string ShipType { get; set; }
        public string Destination { get; set; }
        public double Heading { get; set; }

        public ShipPosition()
        {
            LastUpdate = DateTime.Now;
            ShipName = "Unknown";
            ShipType = "Unknown";
            Destination = "Unknown";
        }
    }
}
