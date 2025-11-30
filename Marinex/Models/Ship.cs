using System;
using System.Collections.Generic;

namespace Marinex.Models
{
    public class Ship
    {
        public int ShipID { get; set; }
        public string ShipName { get; set; }
        public string ShipType { get; set; }
        public string Owner { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; }
        public DateTime? StartVoyage { get; set; }
        
        public string MMSI { get; set; }
        public bool AISEnabled { get; set; }
    }
}
