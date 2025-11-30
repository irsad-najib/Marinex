using System;
using System.Collections.Generic;
using System.Linq;
using Marinex.Models;

namespace Marinex.Services
{
    public class MaintenanceAIService
    {
        private readonly int CRITICAL_DAYS_THRESHOLD;
        private readonly int WARNING_DAYS_THRESHOLD;
        private readonly int NORMAL_DAYS_THRESHOLD;

        private readonly int CRITICAL_ENGINE_HOURS;
        private readonly int WARNING_ENGINE_HOURS;

        private readonly int OLD_SHIP_AGE_YEARS;
        private readonly int MATURE_SHIP_AGE_YEARS;

        private readonly double HIGH_FUEL_DEVIATION;
        private readonly double MODERATE_FUEL_DEVIATION;

        public MaintenanceAIService(
            int criticalDaysThreshold = 180,
            int warningDaysThreshold = 120,
            int normalDaysThreshold = 60,
            int criticalEngineHours = 5000,
            int warningEngineHours = 3500,
            int oldShipAgeYears = 15,
            int matureShipAgeYears = 10,
            double highFuelDeviation = 0.20,
            double moderateFuelDeviation = 0.10)
        {
            CRITICAL_DAYS_THRESHOLD = criticalDaysThreshold;
            WARNING_DAYS_THRESHOLD = warningDaysThreshold;
            NORMAL_DAYS_THRESHOLD = normalDaysThreshold;

            CRITICAL_ENGINE_HOURS = criticalEngineHours;
            WARNING_ENGINE_HOURS = warningEngineHours;

            OLD_SHIP_AGE_YEARS = oldShipAgeYears;
            MATURE_SHIP_AGE_YEARS = matureShipAgeYears;

            HIGH_FUEL_DEVIATION = highFuelDeviation;
            MODERATE_FUEL_DEVIATION = moderateFuelDeviation;
        }

        public MaintenancePrediction PredictMaintenance(Ship ship, List<Maintenance> maintenanceHistory,
                                                       double engineHours, double fuelConsumptionRate,
                                                       MaintenanceReport report = null)
        {
            var prediction = new MaintenancePrediction
            {
                ShipID = ship.ShipID,
                ShipName = ship.ShipName,
                AnalysisDate = DateTime.Now
            };
            int? daysSinceLastMaintenance = CalculateDaysSinceLastMaintenance(maintenanceHistory);

            int shipAge = CalculateShipAge(ship);

            double fuelDeviation = AnalyzeFuelConsumption(fuelConsumptionRate);

            var risk = EvaluateMaintenanceRisk(daysSinceLastMaintenance, engineHours,
                                              shipAge, fuelDeviation);

            if (report != null)
            {
                if (report.IsUrgent())
                    risk = BumpRisk(risk, 1);

                var text = $"{report.EquipmentName} {report.IssueDescription}".ToLower();
                int keywordBumps = 0;
                if (text.Contains("engine") || text.Contains("motor") || text.Contains("oil")) keywordBumps++;
                if (text.Contains("fuel") || text.Contains("injector") || text.Contains("consumption")) keywordBumps++;
                if (text.Contains("hull") || text.Contains("leak") || text.Contains("water")) keywordBumps++;
                if (text.Contains("fire") || text.Contains("smoke") || text.Contains("electrical")) keywordBumps += 2;

                if (keywordBumps > 0)
                    risk = BumpRisk(risk, keywordBumps);
            }

            GenerateRecommendation(prediction, risk, daysSinceLastMaintenance ?? -1, engineHours, shipAge, fuelDeviation);

            if (report != null)
            {
                prediction.Reasoning += $"\n\nReport linked: Equipment={report.EquipmentName}, Priority={report.Priority}, Issue={report.IssueDescription}";
            }
            prediction.DaysSinceLastMaintenance = daysSinceLastMaintenance ?? -1;

            return prediction;
        }
        

        private RiskLevel EvaluateMaintenanceRisk(int? daysSinceLastMaintenance, double engineHours,
                                                   int shipAge, double fuelDeviation)
        {
            if (!daysSinceLastMaintenance.HasValue)
            {
                if (engineHours >= CRITICAL_ENGINE_HOURS || fuelDeviation >= HIGH_FUEL_DEVIATION)
                    return RiskLevel.High;

                if (engineHours >= WARNING_ENGINE_HOURS || shipAge >= OLD_SHIP_AGE_YEARS || fuelDeviation >= MODERATE_FUEL_DEVIATION)
                    return RiskLevel.Moderate;

                return RiskLevel.Low;
            }

            int days = daysSinceLastMaintenance.Value;

            if (days >= CRITICAL_DAYS_THRESHOLD)
                return RiskLevel.Critical;

            if (engineHours >= CRITICAL_ENGINE_HOURS)
                return RiskLevel.Critical;

            if (days >= WARNING_DAYS_THRESHOLD && shipAge >= OLD_SHIP_AGE_YEARS)
                return RiskLevel.Critical;

            if (engineHours >= WARNING_ENGINE_HOURS && fuelDeviation >= HIGH_FUEL_DEVIATION)
                return RiskLevel.Critical;

            if (days >= WARNING_DAYS_THRESHOLD)
                return RiskLevel.High;

            if (engineHours >= WARNING_ENGINE_HOURS)
                return RiskLevel.High;

            if (shipAge >= OLD_SHIP_AGE_YEARS && fuelDeviation >= MODERATE_FUEL_DEVIATION)
                return RiskLevel.High;

            if (days >= NORMAL_DAYS_THRESHOLD && (engineHours >= 2500 || fuelDeviation >= MODERATE_FUEL_DEVIATION))
                return RiskLevel.High;

            if (days >= NORMAL_DAYS_THRESHOLD)
                return RiskLevel.Moderate;

            if (engineHours >= 2000)
                return RiskLevel.Moderate;

            if (shipAge >= MATURE_SHIP_AGE_YEARS)
                return RiskLevel.Moderate;

            if (fuelDeviation >= MODERATE_FUEL_DEVIATION)
                return RiskLevel.Moderate;
            return RiskLevel.Low;
        }

        private RiskLevel BumpRisk(RiskLevel baseRisk, int steps)
        {
            var values = Enum.GetValues(typeof(RiskLevel)).Cast<RiskLevel>().ToList();
            int idx = values.IndexOf(baseRisk);
            idx = Math.Min(values.Count - 1, idx + steps);
            return values[idx];
        }
        
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
        
        
        private int? CalculateDaysSinceLastMaintenance(List<Maintenance> maintenanceHistory)
        {
            if (maintenanceHistory == null || maintenanceHistory.Count == 0)
                return null; 

            var lastMaintenance = maintenanceHistory
                .OrderByDescending(m => m.Date)
                .FirstOrDefault();

            if (lastMaintenance == null)
                return null;

            return (DateTime.Now - lastMaintenance.Date).Days;
        }
        
        private int CalculateShipAge(Ship ship)
        {
            if (ship.StartVoyage.HasValue)
            {
                return DateTime.Now.Year - ship.StartVoyage.Value.Year;
            }
            return 10; 
        }
        
        private double AnalyzeFuelConsumption(double currentRate)
        {
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
                RiskLevel.Critical => 50000m,  
                RiskLevel.High => 25000m,      
                RiskLevel.Moderate => 10000m,
                RiskLevel.Low => 2000m,
                _ => 5000m
            };
            
            decimal additionalCost = (maintenanceTypeCount - 1) * 5000m;
            
            return baseCost + additionalCost;
        }
    }
    
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
