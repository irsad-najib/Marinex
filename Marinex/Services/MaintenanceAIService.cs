using System;
using System.Collections.Generic;
using System.Linq;
using Marinex.Models;

namespace Marinex.Services
{
    /// <summary>
    /// AI-based Maintenance Prediction Service using Rule-Based Decision Tree
    /// No ML training required - uses expert knowledge and threshold-based logic
    /// </summary>
    public class MaintenanceAIService
    {
        // ========== CONFIGURATION THRESHOLDS ==========
        
        // Critical thresholds
        private const int CRITICAL_DAYS_THRESHOLD = 180;  // 6 months
        private const int WARNING_DAYS_THRESHOLD = 120;   // 4 months
        private const int NORMAL_DAYS_THRESHOLD = 60;     // 2 months
        
        private const int CRITICAL_ENGINE_HOURS = 5000;
        private const int WARNING_ENGINE_HOURS = 3500;
        
        private const int OLD_SHIP_AGE_YEARS = 15;
        private const int MATURE_SHIP_AGE_YEARS = 10;
        
        // Fuel consumption deviation (percentage)
        private const double HIGH_FUEL_DEVIATION = 0.20;  // 20% higher than normal
        private const double MODERATE_FUEL_DEVIATION = 0.10; // 10% higher
        
        /// <summary>
        /// Main prediction method - analyzes ship data and returns maintenance recommendation
        /// </summary>
        public MaintenancePrediction PredictMaintenance(Ship ship, List<Maintenance> maintenanceHistory, 
                                                       double engineHours, double fuelConsumptionRate)
        {
            var prediction = new MaintenancePrediction
            {
                ShipID = ship.ShipID,
                ShipName = ship.ShipName,
                AnalysisDate = DateTime.Now
            };
            
            // Step 1: Calculate days since last maintenance
            int daysSinceLastMaintenance = CalculateDaysSinceLastMaintenance(maintenanceHistory);
            
            // Step 2: Calculate ship age
            int shipAge = CalculateShipAge(ship);
            
            // Step 3: Analyze fuel consumption pattern
            double fuelDeviation = AnalyzeFuelConsumption(fuelConsumptionRate);
            
            // Step 4: Decision Tree Logic
            var risk = EvaluateMaintenanceRisk(daysSinceLastMaintenance, engineHours, 
                                              shipAge, fuelDeviation);
            
            // Step 5: Generate recommendation
            GenerateRecommendation(prediction, risk, daysSinceLastMaintenance, 
                                 engineHours, shipAge, fuelDeviation);
            
            return prediction;
        }
        
        /// <summary>
        /// Decision Tree - Core Logic
        /// </summary>
        private RiskLevel EvaluateMaintenanceRisk(int daysSinceLastMaintenance, double engineHours,
                                                   int shipAge, double fuelDeviation)
        {
            // CRITICAL RISK - Immediate action needed
            if (daysSinceLastMaintenance >= CRITICAL_DAYS_THRESHOLD)
                return RiskLevel.Critical;
            
            if (engineHours >= CRITICAL_ENGINE_HOURS)
                return RiskLevel.Critical;
            
            if (daysSinceLastMaintenance >= WARNING_DAYS_THRESHOLD && 
                shipAge >= OLD_SHIP_AGE_YEARS)
                return RiskLevel.Critical;
            
            if (engineHours >= WARNING_ENGINE_HOURS && 
                fuelDeviation >= HIGH_FUEL_DEVIATION)
                return RiskLevel.Critical;
            
            // HIGH RISK - Action needed soon
            if (daysSinceLastMaintenance >= WARNING_DAYS_THRESHOLD)
                return RiskLevel.High;
            
            if (engineHours >= WARNING_ENGINE_HOURS)
                return RiskLevel.High;
            
            if (shipAge >= OLD_SHIP_AGE_YEARS && 
                fuelDeviation >= MODERATE_FUEL_DEVIATION)
                return RiskLevel.High;
            
            if (daysSinceLastMaintenance >= NORMAL_DAYS_THRESHOLD && 
                (engineHours >= 2500 || fuelDeviation >= MODERATE_FUEL_DEVIATION))
                return RiskLevel.High;
            
            // MODERATE RISK - Monitor closely
            if (daysSinceLastMaintenance >= NORMAL_DAYS_THRESHOLD)
                return RiskLevel.Moderate;
            
            if (engineHours >= 2000)
                return RiskLevel.Moderate;
            
            if (shipAge >= MATURE_SHIP_AGE_YEARS)
                return RiskLevel.Moderate;
            
            if (fuelDeviation >= MODERATE_FUEL_DEVIATION)
                return RiskLevel.Moderate;
            
            // LOW RISK - All good
            return RiskLevel.Low;
        }
        
        /// <summary>
        /// Generate detailed recommendation based on risk level
        /// </summary>
        private void GenerateRecommendation(MaintenancePrediction prediction, RiskLevel risk,
                                           int daysSinceLastMaintenance, double engineHours,
                                           int shipAge, double fuelDeviation)
        {
            prediction.RiskLevel = risk;
            prediction.DaysSinceLastMaintenance = daysSinceLastMaintenance;
            prediction.EngineHours = engineHours;
            prediction.ShipAge = shipAge;
            prediction.FuelDeviationPercentage = fuelDeviation * 100;
            
            switch (risk)
            {
                case RiskLevel.Critical:
                    prediction.Priority = "URGENT";
                    prediction.RecommendedAction = "Schedule maintenance IMMEDIATELY";
                    prediction.EstimatedDaysUntilMaintenance = 0;
                    prediction.MaintenanceTypes = GetCriticalMaintenanceTypes(daysSinceLastMaintenance, 
                                                                              engineHours, fuelDeviation);
                    prediction.Reasoning = BuildCriticalReasoning(daysSinceLastMaintenance, engineHours, 
                                                                  shipAge, fuelDeviation);
                    prediction.EstimatedCost = CalculateEstimatedCost(RiskLevel.Critical, prediction.MaintenanceTypes.Count);
                    break;
                    
                case RiskLevel.High:
                    prediction.Priority = "HIGH";
                    prediction.RecommendedAction = "Schedule maintenance within 2 weeks";
                    prediction.EstimatedDaysUntilMaintenance = 14;
                    prediction.MaintenanceTypes = GetHighRiskMaintenanceTypes(daysSinceLastMaintenance, 
                                                                              engineHours, fuelDeviation);
                    prediction.Reasoning = BuildHighRiskReasoning(daysSinceLastMaintenance, engineHours, 
                                                                  shipAge, fuelDeviation);
                    prediction.EstimatedCost = CalculateEstimatedCost(RiskLevel.High, prediction.MaintenanceTypes.Count);
                    break;
                    
                case RiskLevel.Moderate:
                    prediction.Priority = "MEDIUM";
                    prediction.RecommendedAction = "Schedule maintenance within 1 month";
                    prediction.EstimatedDaysUntilMaintenance = 30;
                    prediction.MaintenanceTypes = GetModerateMaintenanceTypes(daysSinceLastMaintenance, 
                                                                               engineHours, fuelDeviation);
                    prediction.Reasoning = BuildModerateReasoning(daysSinceLastMaintenance, engineHours, 
                                                                  shipAge, fuelDeviation);
                    prediction.EstimatedCost = CalculateEstimatedCost(RiskLevel.Moderate, prediction.MaintenanceTypes.Count);
                    break;
                    
                case RiskLevel.Low:
                    prediction.Priority = "LOW";
                    prediction.RecommendedAction = "Continue monitoring - maintenance not required yet";
                    prediction.EstimatedDaysUntilMaintenance = 90;
                    prediction.MaintenanceTypes = new List<string> { "Routine Inspection" };
                    prediction.Reasoning = "All parameters are within normal operating ranges. " +
                                          "Ship is well-maintained and operating efficiently.";
                    prediction.EstimatedCost = CalculateEstimatedCost(RiskLevel.Low, 1);
                    break;
            }
        }
        
        // ========== HELPER METHODS ==========
        
        private int CalculateDaysSinceLastMaintenance(List<Maintenance> maintenanceHistory)
        {
            if (maintenanceHistory == null || maintenanceHistory.Count == 0)
                return 999; // No maintenance history - treat as critical
            
            var lastMaintenance = maintenanceHistory
                .OrderByDescending(m => m.Date)
                .FirstOrDefault();
            
            if (lastMaintenance == null)
                return 999;
            
            return (DateTime.Now - lastMaintenance.Date).Days;
        }
        
        private int CalculateShipAge(Ship ship)
        {
            // Assuming ship has a BuildYear property or we estimate from first voyage
            if (ship.StartVoyage.HasValue)
            {
                return DateTime.Now.Year - ship.StartVoyage.Value.Year;
            }
            return 10; // Default to mature ship if no data
        }
        
        private double AnalyzeFuelConsumption(double currentRate)
        {
            // Compare with baseline (assuming 100 liters/hour as baseline)
            const double BASELINE_CONSUMPTION = 100.0;
            return (currentRate - BASELINE_CONSUMPTION) / BASELINE_CONSUMPTION;
        }
        
        private List<string> GetCriticalMaintenanceTypes(int days, double hours, double fuelDev)
        {
            var types = new List<string>();
            
            if (days >= CRITICAL_DAYS_THRESHOLD)
                types.Add("Complete Engine Overhaul");
            
            if (hours >= CRITICAL_ENGINE_HOURS)
                types.Add("Engine Component Replacement");
            
            if (fuelDev >= HIGH_FUEL_DEVIATION)
            {
                types.Add("Fuel System Inspection & Cleaning");
                types.Add("Injector Replacement");
            }
            
            types.Add("Hull Inspection");
            types.Add("Safety Equipment Check");
            
            return types;
        }
        
        private List<string> GetHighRiskMaintenanceTypes(int days, double hours, double fuelDev)
        {
            var types = new List<string>();
            
            if (days >= WARNING_DAYS_THRESHOLD)
                types.Add("Engine Maintenance");
            
            if (hours >= WARNING_ENGINE_HOURS)
                types.Add("Engine Oil Change");
            
            if (fuelDev >= MODERATE_FUEL_DEVIATION)
                types.Add("Fuel Filter Replacement");
            
            types.Add("Propeller Inspection");
            types.Add("Electrical System Check");
            
            return types;
        }
        
        private List<string> GetModerateMaintenanceTypes(int days, double hours, double fuelDev)
        {
            var types = new List<string>();
            
            types.Add("Routine Inspection");
            types.Add("Lubrication Service");
            
            if (hours >= 2000)
                types.Add("Filter Replacement");
            
            if (fuelDev >= MODERATE_FUEL_DEVIATION)
                types.Add("Fuel System Check");
            
            return types;
        }
        
        private string BuildCriticalReasoning(int days, double hours, int age, double fuelDev)
        {
            var reasons = new List<string>();
            
            if (days >= CRITICAL_DAYS_THRESHOLD)
                reasons.Add($"⚠️ {days} days since last maintenance (Critical threshold: {CRITICAL_DAYS_THRESHOLD})");
            
            if (hours >= CRITICAL_ENGINE_HOURS)
                reasons.Add($"⚠️ Engine hours at {hours:F0} (Critical threshold: {CRITICAL_ENGINE_HOURS})");
            
            if (age >= OLD_SHIP_AGE_YEARS)
                reasons.Add($"⚠️ Ship age: {age} years (Requires frequent maintenance)");
            
            if (fuelDev >= HIGH_FUEL_DEVIATION)
                reasons.Add($"⚠️ Fuel consumption {fuelDev*100:F1}% above normal (Indicates engine issues)");
            
            return "CRITICAL SITUATION:\n" + string.Join("\n", reasons) + 
                   "\n\nImmediate maintenance required to prevent breakdown and ensure safety.";
        }
        
        private string BuildHighRiskReasoning(int days, double hours, int age, double fuelDev)
        {
            var reasons = new List<string>();
            
            if (days >= WARNING_DAYS_THRESHOLD)
                reasons.Add($"⚡ {days} days since last maintenance (Warning threshold: {WARNING_DAYS_THRESHOLD})");
            
            if (hours >= WARNING_ENGINE_HOURS)
                reasons.Add($"⚡ Engine hours at {hours:F0} (Warning threshold: {WARNING_ENGINE_HOURS})");
            
            if (age >= OLD_SHIP_AGE_YEARS)
                reasons.Add($"⚡ Older vessel ({age} years) requires more frequent checks");
            
            if (fuelDev >= MODERATE_FUEL_DEVIATION)
                reasons.Add($"⚡ Elevated fuel consumption (+{fuelDev*100:F1}%)");
            
            return "HIGH RISK:\n" + string.Join("\n", reasons) + 
                   "\n\nMaintenance should be scheduled soon to prevent escalation.";
        }
        
        private string BuildModerateReasoning(int days, double hours, int age, double fuelDev)
        {
            var reasons = new List<string>();
            
            if (days >= NORMAL_DAYS_THRESHOLD)
                reasons.Add($"📅 {days} days since last maintenance (Normal interval reached)");
            
            if (hours >= 2000)
                reasons.Add($"🔧 Engine hours at {hours:F0} (Approaching service interval)");
            
            if (age >= MATURE_SHIP_AGE_YEARS)
                reasons.Add($"📊 Mature vessel ({age} years) - preventive care recommended");
            
            if (fuelDev >= MODERATE_FUEL_DEVIATION)
                reasons.Add($"⛽ Fuel consumption slightly elevated (+{fuelDev*100:F1}%)");
            
            if (reasons.Count == 0)
                reasons.Add("📋 Routine maintenance interval approaching");
            
            return "MODERATE MONITORING:\n" + string.Join("\n", reasons) + 
                   "\n\nPreventive maintenance recommended within next month.";
        }
        
        private decimal CalculateEstimatedCost(RiskLevel risk, int maintenanceTypeCount)
        {
            decimal baseCost = risk switch
            {
                RiskLevel.Critical => 50000m,  // $50,000
                RiskLevel.High => 25000m,      // $25,000
                RiskLevel.Moderate => 10000m,  // $10,000
                RiskLevel.Low => 2000m,        // $2,000
                _ => 5000m
            };
            
            // Add cost per maintenance type
            decimal additionalCost = (maintenanceTypeCount - 1) * 5000m;
            
            return baseCost + additionalCost;
        }
    }
    
    // ========== SUPPORTING CLASSES ==========
    
    public enum RiskLevel
    {
        Low,
        Moderate,
        High,
        Critical
    }
    
    public class MaintenancePrediction
    {
        public int ShipID { get; set; }
        public string ShipName { get; set; }
        public DateTime AnalysisDate { get; set; }
        
        // Risk Assessment
        public RiskLevel RiskLevel { get; set; }
        public string Priority { get; set; }  // LOW, MEDIUM, HIGH, URGENT
        
        // Metrics
        public int DaysSinceLastMaintenance { get; set; }
        public double EngineHours { get; set; }
        public int ShipAge { get; set; }
        public double FuelDeviationPercentage { get; set; }
        
        // Recommendations
        public string RecommendedAction { get; set; }
        public int EstimatedDaysUntilMaintenance { get; set; }
        public List<string> MaintenanceTypes { get; set; } = new();
        public string Reasoning { get; set; }
        public decimal EstimatedCost { get; set; }
        
        // Display helpers
        public string GetRiskBadgeColor()
        {
            return RiskLevel switch
            {
                RiskLevel.Critical => "#D32F2F",  // Red
                RiskLevel.High => "#F57C00",      // Orange
                RiskLevel.Moderate => "#FBC02D",  // Yellow
                RiskLevel.Low => "#388E3C",       // Green
                _ => "#757575"
            };
        }
        
        public string GetRiskIcon()
        {
            return RiskLevel switch
            {
                RiskLevel.Critical => "🚨",
                RiskLevel.High => "⚠️",
                RiskLevel.Moderate => "⚡",
                RiskLevel.Low => "✅",
                _ => "ℹ️"
            };
        }
        
        public override string ToString()
        {
            return $"{GetRiskIcon()} {Priority} Priority - {RecommendedAction}\n" +
                   $"Risk Level: {RiskLevel}\n" +
                   $"Estimated Cost: ${EstimatedCost:N0}\n" +
                   $"Maintenance Types: {string.Join(", ", MaintenanceTypes)}\n" +
                   $"\n{Reasoning}";
        }
    }
}
