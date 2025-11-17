using System;
using System.Collections.Generic;

namespace Marinex.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Company { get; set; }
        public DateTime? LogIn { get; set; }
        public DateTime? LogOut { get; set; }
        public bool? SubmitReport { get; set; }

        // Navigasi
        public List<WeatherReport> Reports { get; set; } = new();
        public List<UserShip> UserShips { get; set; } = new();
        public List<Voyage> Voyages { get; set; } = new();
        public List<Maintenance> Maintenances { get; set; } = new();
        public List<SafetyReport> SafetyReports { get; set; } = new();
        public List<PollutionReport> PollutionReports { get; set; } = new();

        // Methods
        public void Login()
        {
            LogIn = DateTime.Now;
        }

        public void Logout()
        {
            LogOut = DateTime.Now;
        }

        public bool CanSubmitReport()
        {
            return SubmitReport ?? false;
        }
    }
}
