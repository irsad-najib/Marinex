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
        public DateTime? EndVoyage { get; set; }
        
        // AIS Tracking Enhancement
        public string MMSI { get; set; } // Maritime Mobile Service Identity (9 digits)
        public bool AISEnabled { get; set; }
        public DateTime? LastPositionUpdate { get; set; }
        public double? CurrentLatitude { get; set; }
        public double? CurrentLongitude { get; set; }
        public double? CurrentSpeed { get; set; }
        public double? CurrentCourse { get; set; }
        public double? CurrentHeading { get; set; }

        // Navigasi
        public List<UserShip> UserShips { get; set; } = new();
        public List<Voyage> Voyages { get; set; } = new();
        public List<Maintenance> Maintenances { get; set; } = new();
        public List<ShipPosition> PositionHistory { get; set; } = new();

        // Methods sesuai diagram
        public void StartVoyageMethod() 
        { 
            Status = "Sailing";
            StartVoyage = DateTime.Now;
        }
        
        public void EndVoyageMethod() 
        { 
            Status = "Docked";
            EndVoyage = DateTime.Now;
        }
        
        // AIS Tracking Methods
        public void UpdatePosition(double lat, double lon, double speed, double course, double heading)
        {
            CurrentLatitude = lat;
            CurrentLongitude = lon;
            CurrentSpeed = speed;
            CurrentCourse = course;
            CurrentHeading = heading;
            LastPositionUpdate = DateTime.Now;
        }
        
        public bool IsTracking()
        {
            return AISEnabled && !string.IsNullOrEmpty(MMSI);
        }
        
        public string GetCurrentPositionString()
        {
            if (CurrentLatitude.HasValue && CurrentLongitude.HasValue)
                return $"{CurrentLatitude:F6}, {CurrentLongitude:F6}";
            return "Position unknown";
        }
    }
}
