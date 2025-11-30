using System;
using System.Collections.Generic;
using Marinex.Models;
using Marinex.Services;

namespace Marinex.Tests
{
    /// <summary>
    /// Demo & Test untuk AI Maintenance Prediction System
    /// Tidak perlu unit test framework - bisa langsung di-run
    /// NOTE: Ini bukan Main entry point, hanya collection of test scenarios
    /// </summary>
    public class MaintenanceAIDemo
    {
        /// <summary>
        /// Run all test scenarios - call this from your main program or test
        /// </summary>
        public static void RunAllTests()
        {
            Console.WriteLine("=" .PadRight(80, '='));
            Console.WriteLine("🤖 MARINEX AI MAINTENANCE PREDICTION SYSTEM - DEMO");
            Console.WriteLine("=" .PadRight(80, '='));
            Console.WriteLine();
            
            var aiService = new MaintenanceAIService();
            
            // Run all test scenarios
            TestScenario1_CriticalOverdue(aiService);
            TestScenario2_HighRisk(aiService);
            TestScenario3_ModerateRisk(aiService);
            TestScenario4_LowRisk(aiService);
            TestScenario5_OldShipWithIssues(aiService);
            TestScenario6_NewShipHighUsage(aiService);
            
            Console.WriteLine();
            Console.WriteLine("=" .PadRight(80, '='));
            Console.WriteLine("✅ All demo scenarios completed!");
            Console.WriteLine("=" .PadRight(80, '='));
        }
        
        /// <summary>
        /// Run specific test scenario by number
        /// </summary>
        public static void RunTestScenario(int scenarioNumber)
        {
            var aiService = new MaintenanceAIService();
            
            switch (scenarioNumber)
            {
                case 1:
                    TestScenario1_CriticalOverdue(aiService);
                    break;
                case 2:
                    TestScenario2_HighRisk(aiService);
                    break;
                case 3:
                    TestScenario3_ModerateRisk(aiService);
                    break;
                case 4:
                    TestScenario4_LowRisk(aiService);
                    break;
                case 5:
                    TestScenario5_OldShipWithIssues(aiService);
                    break;
                case 6:
                    TestScenario6_NewShipHighUsage(aiService);
                    break;
                default:
                    Console.WriteLine($"Test scenario {scenarioNumber} not found. Valid range: 1-6");
                    break;
            }
        }
        
        /// <summary>
        /// Scenario 1: CRITICAL - Maintenance overdue lebih dari 6 bulan
        /// </summary>
        private static void TestScenario1_CriticalOverdue(MaintenanceAIService aiService)
        {
            PrintScenarioHeader("Scenario 1: CRITICAL - Overdue Maintenance");
            
            var ship = CreateTestShip("MV Pacific Pioneer", 2008);
            
            var maintenanceHistory = new List<Maintenance>
            {
                new Maintenance 
                { 
                    Date = DateTime.Now.AddDays(-200), // 200 days ago - OVERDUE!
                    Type = "Engine Service",
                    Status = "Done"
                }
            };
            
            double engineHours = 4800;
            double fuelRate = 125; // 25% above normal - indicator masalah
            
            var prediction = aiService.PredictMaintenance(ship, maintenanceHistory, engineHours, fuelRate);
            PrintPrediction(prediction);
            
            // Validation
            if (prediction.RiskLevel == RiskLevel.Critical && prediction.Priority == "URGENT")
                Console.WriteLine("✅ TEST PASSED: Correctly identified as CRITICAL\n");
            else
                Console.WriteLine("❌ TEST FAILED: Should be CRITICAL\n");
        }
        
        /// <summary>
        /// Scenario 2: HIGH RISK - Engine hours tinggi + fuel consumption naik
        /// </summary>
        private static void TestScenario2_HighRisk(MaintenanceAIService aiService)
        {
            PrintScenarioHeader("Scenario 2: HIGH RISK - High Engine Hours");
            
            var ship = CreateTestShip("MV Atlantic Explorer", 2012);
            
            var maintenanceHistory = new List<Maintenance>
            {
                new Maintenance 
                { 
                    Date = DateTime.Now.AddDays(-100),
                    Type = "Routine Service",
                    Status = "Done"
                }
            };
            
            double engineHours = 3700; // Above warning threshold
            double fuelRate = 112; // 12% above normal
            
            var prediction = aiService.PredictMaintenance(ship, maintenanceHistory, engineHours, fuelRate);
            PrintPrediction(prediction);
            
            if (prediction.RiskLevel == RiskLevel.High && prediction.EstimatedDaysUntilMaintenance <= 14)
                Console.WriteLine("✅ TEST PASSED: Correctly identified as HIGH RISK\n");
            else
                Console.WriteLine("❌ TEST FAILED: Should be HIGH RISK\n");
        }
        
        /// <summary>
        /// Scenario 3: MODERATE - Approaching routine maintenance interval
        /// </summary>
        private static void TestScenario3_ModerateRisk(MaintenanceAIService aiService)
        {
            PrintScenarioHeader("Scenario 3: MODERATE - Routine Interval");
            
            var ship = CreateTestShip("MV Indian Voyager", 2015);
            
            var maintenanceHistory = new List<Maintenance>
            {
                new Maintenance 
                { 
                    Date = DateTime.Now.AddDays(-70), // 70 days - approaching 2 month mark
                    Type = "Engine Service",
                    Status = "Done"
                }
            };
            
            double engineHours = 2300;
            double fuelRate = 105; // Slightly elevated
            
            var prediction = aiService.PredictMaintenance(ship, maintenanceHistory, engineHours, fuelRate);
            PrintPrediction(prediction);
            
            if (prediction.RiskLevel == RiskLevel.Moderate && prediction.Priority == "MEDIUM")
                Console.WriteLine("✅ TEST PASSED: Correctly identified as MODERATE\n");
            else
                Console.WriteLine("❌ TEST FAILED: Should be MODERATE\n");
        }
        
        /// <summary>
        /// Scenario 4: LOW RISK - Everything optimal
        /// </summary>
        private static void TestScenario4_LowRisk(MaintenanceAIService aiService)
        {
            PrintScenarioHeader("Scenario 4: LOW RISK - All Good");
            
            var ship = CreateTestShip("MV Nordic Star", 2020);
            
            var maintenanceHistory = new List<Maintenance>
            {
                new Maintenance 
                { 
                    Date = DateTime.Now.AddDays(-25), // Recent maintenance
                    Type = "Complete Service",
                    Status = "Done"
                }
            };
            
            double engineHours = 1200; // Low usage
            double fuelRate = 98; // Below baseline - efficient!
            
            var prediction = aiService.PredictMaintenance(ship, maintenanceHistory, engineHours, fuelRate);
            PrintPrediction(prediction);
            
            if (prediction.RiskLevel == RiskLevel.Low && prediction.Priority == "LOW")
                Console.WriteLine("✅ TEST PASSED: Correctly identified as LOW RISK\n");
            else
                Console.WriteLine("❌ TEST FAILED: Should be LOW RISK\n");
        }
        
        /// <summary>
        /// Scenario 5: CRITICAL - Old ship + multiple issues
        /// </summary>
        private static void TestScenario5_OldShipWithIssues(MaintenanceAIService aiService)
        {
            PrintScenarioHeader("Scenario 5: CRITICAL - Old Ship Multiple Issues");
            
            var ship = CreateTestShip("MV Heritage Queen", 2005); // 20 years old
            
            var maintenanceHistory = new List<Maintenance>
            {
                new Maintenance 
                { 
                    Date = DateTime.Now.AddDays(-130),
                    Type = "Engine Repair",
                    Status = "Done"
                }
            };
            
            double engineHours = 4200;
            double fuelRate = 122; // 22% above - serious issue
            
            var prediction = aiService.PredictMaintenance(ship, maintenanceHistory, engineHours, fuelRate);
            PrintPrediction(prediction);
            
            if (prediction.RiskLevel == RiskLevel.Critical)
                Console.WriteLine("✅ TEST PASSED: Old ship with issues = CRITICAL\n");
            else
                Console.WriteLine("❌ TEST FAILED: Should be CRITICAL\n");
        }
        
        /// <summary>
        /// Scenario 6: MODERATE - New ship but heavy usage
        /// </summary>
        private static void TestScenario6_NewShipHighUsage(MaintenanceAIService aiService)
        {
            PrintScenarioHeader("Scenario 6: MODERATE - New Ship Heavy Usage");
            
            var ship = CreateTestShip("MV Future Vision", 2023); // Brand new
            
            var maintenanceHistory = new List<Maintenance>
            {
                new Maintenance 
                { 
                    Date = DateTime.Now.AddDays(-45),
                    Type = "Initial Service",
                    Status = "Done"
                }
            };
            
            double engineHours = 2100; // High usage for new ship
            double fuelRate = 102; // Normal
            
            var prediction = aiService.PredictMaintenance(ship, maintenanceHistory, engineHours, fuelRate);
            PrintPrediction(prediction);
            
            if (prediction.RiskLevel <= RiskLevel.Moderate)
                Console.WriteLine("✅ TEST PASSED: New ship = Lower risk despite usage\n");
            else
                Console.WriteLine("⚠️ TEST WARNING: New ship showing higher risk than expected\n");
        }
        
        // ========== HELPER METHODS ==========
        
        private static Ship CreateTestShip(string name, int buildYear)
        {
            return new Ship
            {
                ShipID = new Random().Next(1000, 9999),
                ShipName = name,
                ShipType = "Container Ship",
                Owner = "Test Fleet Co.",
                Status = "Sailing",
                StartVoyage = new DateTime(buildYear, 1, 1)
            };
        }
        
        private static void PrintScenarioHeader(string title)
        {
            Console.WriteLine();
            Console.WriteLine("━".PadRight(80, '━'));
            Console.WriteLine($"📋 {title}");
            Console.WriteLine("━".PadRight(80, '━'));
        }
        
        private static void PrintPrediction(MaintenancePrediction prediction)
        {
            Console.WriteLine();
            Console.WriteLine($"Ship: {prediction.ShipName}");
            Console.WriteLine($"Analysis Date: {prediction.AnalysisDate:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine();
            
            // Risk badge
            Console.ForegroundColor = prediction.RiskLevel switch
            {
                RiskLevel.Critical => ConsoleColor.Red,
                RiskLevel.High => ConsoleColor.Yellow,
                RiskLevel.Moderate => ConsoleColor.Cyan,
                RiskLevel.Low => ConsoleColor.Green,
                _ => ConsoleColor.White
            };
            Console.WriteLine($"{prediction.GetRiskIcon()} RISK LEVEL: {prediction.RiskLevel.ToString().ToUpper()} ({prediction.Priority} Priority)");
            Console.ResetColor();
            Console.WriteLine();
            
            // Metrics
            Console.WriteLine("📊 METRICS:");
            Console.WriteLine($"   • Days Since Last Maintenance: {prediction.DaysSinceLastMaintenance}");
            Console.WriteLine($"   • Engine Hours: {prediction.EngineHours:F0}");
            Console.WriteLine($"   • Ship Age: {prediction.ShipAge} years");
            Console.WriteLine($"   • Fuel Deviation: {prediction.FuelDeviationPercentage:+0.0;-0.0;0}%");
            Console.WriteLine();
            
            // Recommendation
            Console.WriteLine("💡 RECOMMENDATION:");
            Console.WriteLine($"   {prediction.RecommendedAction}");
            Console.WriteLine($"   Estimated Days: {prediction.EstimatedDaysUntilMaintenance}");
            Console.WriteLine();
            
            // Maintenance types
            Console.WriteLine("🔧 REQUIRED MAINTENANCE:");
            foreach (var type in prediction.MaintenanceTypes)
            {
                Console.WriteLine($"   • {type}");
            }
            Console.WriteLine();
            
            // Cost
            Console.WriteLine($"💰 ESTIMATED COST: ${prediction.EstimatedCost:N0}");
            Console.WriteLine();
            
            // Reasoning
            Console.WriteLine("📝 REASONING:");
            var reasoningLines = prediction.Reasoning.Split('\n');
            foreach (var line in reasoningLines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                    Console.WriteLine($"   {line}");
            }
            Console.WriteLine();
        }
    }
}
