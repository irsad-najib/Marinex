using System;
using System.Collections.Generic;
using System.Linq;
using Marinex.Models;

namespace Marinex.Services
{
    /// <summary>
    /// Helper service untuk generate data simulasi
    /// Dipakai saat belum ada sensor atau data real
    /// </summary>
    public class DataSimulatorService
    {
        private Random _random = new Random();
        
        // ========== SENSOR DATA SIMULATION ==========
        
        /// <summary>
        /// Simulate engine temperature (normal: 75-85°C)
        /// </summary>
        public double GetEngineTemperature(bool hasIssue = false)
        {
            if (hasIssue)
                return _random.Next(90, 105); // Overheating
            return _random.Next(75, 86);
        }
        
        /// <summary>
        /// Simulate fuel level (0-100%)
        /// </summary>
        public double GetFuelLevel()
        {
            return _random.Next(20, 101);
        }
        
        /// <summary>
        /// Simulate engine RPM (normal: 800-1500)
        /// </summary>
        public double GetEngineRPM(bool isHighSpeed = false)
        {
            if (isHighSpeed)
                return _random.Next(1400, 1800);
            return _random.Next(800, 1501);
        }
        
        /// <summary>
        /// Simulate oil pressure (normal: 40-80 PSI)
        /// </summary>
        public double GetOilPressure(bool hasIssue = false)
        {
            if (hasIssue)
                return _random.Next(15, 35); // Low pressure warning
            return _random.Next(40, 81);
        }
        
        /// <summary>
        /// Simulate engine hours based on ship age
        /// </summary>
        public double GetEngineHours(int shipAgeYears)
        {
            // Rough estimate: 400 hours per year average
            double baseHours = shipAgeYears * 400;
            double variation = _random.Next(-100, 201);
            return Math.Max(0, baseHours + variation);
        }
        
        /// <summary>
        /// Simulate fuel consumption rate (liters/hour)
        /// Baseline: 100 L/h for medium cargo ship
        /// </summary>
        public double GetFuelConsumptionRate(bool isEfficient = true)
        {
            if (isEfficient)
                return _random.Next(85, 105); // Good efficiency
            else
                return _random.Next(110, 130); // Poor efficiency - needs maintenance
        }
        
        /// <summary>
        /// Simulate vibration level (0-10 scale)
        /// </summary>
        public double GetVibrationLevel(bool hasIssue = false)
        {
            if (hasIssue)
                return _random.NextDouble() * 4 + 6; // 6-10 = high vibration
            return _random.NextDouble() * 3 + 1; // 1-4 = normal
        }
        
        // ========== MAINTENANCE HISTORY SIMULATION ==========
        
        /// <summary>
        /// Generate fake maintenance history untuk testing AI
        /// </summary>
        public List<Maintenance> GenerateMaintenanceHistory(int shipId, int recordCount, int daysSinceLastMaintenance)
        {
            var history = new List<Maintenance>();
            var currentDate = DateTime.Now.AddDays(-daysSinceLastMaintenance);
            
            var maintenanceTypes = new[]
            {
                "Engine Service",
                "Oil Change",
                "Filter Replacement",
                "Hull Inspection",
                "Propeller Maintenance",
                "Electrical Check",
                "Safety Equipment Inspection",
                "Complete Overhaul"
            };
            
            for (int i = 0; i < recordCount; i++)
            {
                history.Add(new Maintenance
                {
                    MaintenanceID = 1000 + i,
                    Date = currentDate,
                    Type = maintenanceTypes[_random.Next(maintenanceTypes.Length)],
                    Status = "Done",
                    ShipID = shipId
                });
                
                // Previous maintenance was 60-90 days before
                currentDate = currentDate.AddDays(-_random.Next(60, 91));
            }
            
            return history.OrderByDescending(m => m.Date).ToList();
        }
        
        // ========== WEATHER SIMULATION ==========
        
        /// <summary>
        /// Generate fake weather data untuk testing
        /// </summary>
        public (double temperature, string condition, double windSpeed) GenerateWeather()
        {
            var conditions = new[] { "Clear", "Cloudy", "Rain", "Storm", "Fog" };
            var temp = _random.Next(15, 35);
            var condition = conditions[_random.Next(conditions.Length)];
            var windSpeed = _random.Next(5, 40);
            
            return (temp, condition, windSpeed);
        }
        
        // ========== SHIP POSITION SIMULATION ==========
        
        /// <summary>
        /// Generate fake ship position near a starting point
        /// </summary>
        public (double latitude, double longitude) GenerateNearbyPosition(double baseLat, double baseLon, double radiusKm = 100)
        {
            // Simple random offset within radius
            double latOffset = (_random.NextDouble() - 0.5) * (radiusKm / 111.0); // 1 degree = ~111km
            double lonOffset = (_random.NextDouble() - 0.5) * (radiusKm / 111.0);
            
            return (baseLat + latOffset, baseLon + lonOffset);
        }
        
        // ========== COMPREHENSIVE SHIP SIMULATION ==========
        
        /// <summary>
        /// Generate complete simulated ship data untuk demo
        /// </summary>
        public ShipSimulationData GenerateShipSimulation(Ship ship, bool hasProblems = false)
        {
            int shipAge = DateTime.Now.Year - (ship.StartVoyage?.Year ?? DateTime.Now.Year - 10);
            
            return new ShipSimulationData
            {
                Ship = ship,
                EngineHours = GetEngineHours(shipAge),
                EngineTemperature = GetEngineTemperature(hasProblems),
                EngineRPM = GetEngineRPM(false),
                FuelLevel = GetFuelLevel(),
                FuelConsumptionRate = GetFuelConsumptionRate(!hasProblems),
                OilPressure = GetOilPressure(hasProblems),
                VibrationLevel = GetVibrationLevel(hasProblems),
                MaintenanceHistory = GenerateMaintenanceHistory(
                    ship.ShipID, 
                    recordCount: 5, 
                    daysSinceLastMaintenance: hasProblems ? 150 : 45
                ),
                HasIssues = hasProblems
            };
        }
    }
    
    /// <summary>
    /// Data model untuk ship simulation
    /// </summary>
    public class ShipSimulationData
    {
        public Ship Ship { get; set; }
        public double EngineHours { get; set; }
        public double EngineTemperature { get; set; }
        public double EngineRPM { get; set; }
        public double FuelLevel { get; set; }
        public double FuelConsumptionRate { get; set; }
        public double OilPressure { get; set; }
        public double VibrationLevel { get; set; }
        public List<Maintenance> MaintenanceHistory { get; set; }
        public bool HasIssues { get; set; }
        
        public override string ToString()
        {
            return $"Ship: {Ship.ShipName}\n" +
                   $"Engine Hours: {EngineHours:F0}\n" +
                   $"Temperature: {EngineTemperature:F1}°C\n" +
                   $"RPM: {EngineRPM:F0}\n" +
                   $"Fuel Level: {FuelLevel:F1}%\n" +
                   $"Fuel Rate: {FuelConsumptionRate:F1} L/h\n" +
                   $"Oil Pressure: {OilPressure:F1} PSI\n" +
                   $"Vibration: {VibrationLevel:F1}/10\n" +
                   $"Status: {(HasIssues ? "⚠️ Issues Detected" : "✅ Normal")}";
        }
    }
}
