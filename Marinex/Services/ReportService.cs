using System;
using System.Collections.Generic;
using Marinex.Models;

namespace Marinex.Services
{
    // ===========================================
    // ACCESS MODIFIER: PUBLIC - Class bisa diakses dari assembly lain
    // ===========================================
    public class ReportService
    {
        // ===========================================
        // ACCESS MODIFIER: PRIVATE - Field hanya bisa diakses dalam class ini
        // ===========================================
        private int _totalProcessedReports;  // <-- ACCESS MODIFIER: private
        private static int _instanceCount;  // <-- ACCESS MODIFIER: private static
        
        // ===========================================
        // ACCESS MODIFIER: INTERNAL - Bisa diakses dalam assembly yang sama
        // ===========================================
        // Internal method bisa diakses dari class lain dalam assembly yang sama (Marinex)
        // Tapi tidak bisa diakses dari assembly lain
        internal int GetTotalProcessedReports()  // <-- ACCESS MODIFIER: internal
        {
            return _totalProcessedReports;
        }
        
        internal void IncrementProcessedReports()  // <-- ACCESS MODIFIER: internal
        {
            _totalProcessedReports++;
        }
        
        internal static int GetInstanceCount()  // <-- ACCESS MODIFIER: internal static
        {
            return _instanceCount;
        }
        
        public ReportService()
        {
            _instanceCount++;
            _totalProcessedReports = 0;
        }
        /// <summary>
        /// ===========================================
        /// POLYMORPHISM: Method yang menerima base class
        /// ===========================================
        /// Method ini bisa bekerja dengan berbagai child class (SafetyReport, MaintenanceReport, WeatherReport)
        /// karena mereka inherit dari BaseReport
        /// </summary>
        public string ProcessReport(BaseReport report)  // <-- INHERITANCE & POLYMORPHISM: Parameter bertipe BaseReport tapi bisa menerima child classes
        {
            // POLYMORPHISM: Method ini bisa menerima SafetyReport, MaintenanceReport, atau WeatherReport
            // karena mereka semua inherit dari BaseReport
            if (!report.Validate())  // <-- POLYMORPHISM: Validate() dipanggil sesuai tipe sebenarnya
            {
                return "Report validation failed";
            }

            // ===========================================
            // POLYMORPHISM: Runtime Polymorphism - Method Overriding
            // ===========================================
            // GenerateReport() akan dipanggil sesuai dengan tipe sebenarnya:
            // - Jika report adalah SafetyReport -> memanggil SafetyReport.GenerateReport()
            // - Jika report adalah MaintenanceReport -> memanggil MaintenanceReport.GenerateReport()
            // - Jika report adalah WeatherReport -> memanggil WeatherReport.GenerateReport()
            return report.GenerateReport();  // <-- POLYMORPHISM: Method dipanggil sesuai tipe sebenarnya (runtime binding)
        }

        /// <summary>
        /// ===========================================
        /// POLYMORPHISM: Method yang bisa bekerja dengan berbagai implementasi
        /// ===========================================
        /// </summary>
        public string GetReportSummary(BaseReport report)  // <-- INHERITANCE & POLYMORPHISM: Parameter bertipe BaseReport tapi bisa menerima child classes
        {
            // ===========================================
            // POLYMORPHISM: Runtime Polymorphism - Method Overriding
            // ===========================================
            // GetSummary() akan dipanggil sesuai dengan override-nya:
            // - SafetyReport.GetSummary() -> implementasi SafetyReport
            // - MaintenanceReport.GetSummary() -> implementasi MaintenanceReport
            // - WeatherReport.GetSummary() -> implementasi WeatherReport
            return report.GetSummary();  // <-- POLYMORPHISM: Method dipanggil sesuai tipe sebenarnya (runtime binding)
        }

        /// <summary>
        /// Method khusus untuk SafetyReport - masih menggunakan Inheritance
        /// </summary>
        public bool CheckEmergencyResponse(SafetyReport safetyReport)
        {
            return safetyReport.RequiresEmergencyResponse();
        }

        /// <summary>
        /// Method khusus untuk MaintenanceReport - masih menggunakan Inheritance
        /// </summary>
        public bool CheckUrgency(MaintenanceReport maintenanceReport)
        {
            return maintenanceReport.IsUrgent();
        }

        /// <summary>
        /// ===========================================
        /// POLYMORPHISM: Method dengan collection
        /// ===========================================
        /// Bisa menerima berbagai jenis report dan memprosesnya secara polymorphic
        /// </summary>
        public List<string> ProcessMultipleReports(List<BaseReport> reports)  // <-- INHERITANCE & POLYMORPHISM: Parameter bertipe List<BaseReport> tapi bisa menerima child classes
        {
            var results = new List<string>();
            
            // ===========================================
            // POLYMORPHISM: Loop melalui berbagai jenis report
            // ===========================================
            // Semua report di-treat sebagai BaseReport (polymorphism)
            foreach (var report in reports)  // <-- INHERITANCE: report bertipe BaseReport (base class)
            {
                // ===========================================
                // POLYMORPHISM: Runtime Polymorphism - Method Overriding
                // ===========================================
                // GenerateReport() akan dipanggil sesuai tipe sebenarnya:
                // - SafetyReport.GenerateReport() -> implementasi SafetyReport
                // - MaintenanceReport.GenerateReport() -> implementasi MaintenanceReport
                // - WeatherReport.GenerateReport() -> implementasi WeatherReport
                results.Add(report.GenerateReport());  // <-- POLYMORPHISM: Method dipanggil sesuai tipe sebenarnya (runtime binding)
            }
            
            return results;
        }

        /// <summary>
        /// ===========================================
        /// METHOD UTAMA: Menerapkan ketiga konsep OOP
        /// ===========================================
        /// Method comprehensive yang menerapkan Inheritance, Encapsulation, dan Polymorphism
        /// Method ini mengelola dan memproses berbagai jenis report dengan konsep OOP lengkap
        /// </summary>
        /// <param name="reports">List berisi berbagai jenis report (SafetyReport, MaintenanceReport, WeatherReport)</param>
        /// <returns>Dictionary berisi hasil pemrosesan dengan kategori</returns>
        public Dictionary<string, object> ProcessReportsWithOOPConcepts(List<BaseReport> reports)  // <-- INHERITANCE & POLYMORPHISM: Parameter bertipe List<BaseReport> tapi bisa menerima child classes
        {
            // ===========================================
            // ENCAPSULATION: Private/local variables
            // ===========================================
            // Variabel-variabel ini hanya bisa diakses dalam method ini (data hiding)
            var processedCount = 0;
            var validCount = 0;
            var invalidCount = 0;
            var urgentReports = new List<string>();
            var emergencyReports = new List<string>();
            var severeWeatherReports = new List<string>();
            var allReportDetails = new List<string>();

            // ===========================================
            // INHERITANCE & POLYMORPHISM: Loop melalui berbagai jenis report
            // ===========================================
            // Semua report di-treat sebagai BaseReport (polymorphism)
            // Tapi behavior-nya berbeda sesuai tipe sebenarnya (runtime polymorphism)
            foreach (var report in reports)  // <-- INHERITANCE: report bertipe BaseReport (base class)
            {
                processedCount++;

                // ===========================================
                // POLYMORPHISM: Runtime Polymorphism - Method Overriding
                // ===========================================
                // Validate() dipanggil sesuai implementasi tiap child class
                // - SafetyReport.Validate() -> memanggil override di SafetyReport (jika ada) atau base
                // - MaintenanceReport.Validate() -> memanggil override di MaintenanceReport (jika ada) atau base
                // - WeatherReport.Validate() -> memanggil override di WeatherReport (ada validasi tambahan)
                if (report.Validate())  // <-- POLYMORPHISM: Method dipanggil sesuai tipe sebenarnya (runtime binding)
                {
                    validCount++;
                    
                    // ===========================================
                    // POLYMORPHISM: Runtime Polymorphism - Method Overriding
                    // ===========================================
                    // GenerateReport() dipanggil sesuai tipe sebenarnya
                    // - SafetyReport.GenerateReport() -> implementasi SafetyReport
                    // - MaintenanceReport.GenerateReport() -> implementasi MaintenanceReport
                    // - WeatherReport.GenerateReport() -> implementasi WeatherReport
                    string reportContent = report.GenerateReport();  // <-- POLYMORPHISM: Method dipanggil sesuai tipe sebenarnya (runtime binding)
                    allReportDetails.Add(reportContent);

                    // ===========================================
                    // INHERITANCE & POLYMORPHISM: Type checking untuk method khusus
                    // ===========================================
                    // Setiap child class memiliki method khusus yang berbeda
                    // Ini menunjukkan bahwa meskipun semua inherit dari BaseReport,
                    // masing-masing memiliki behavior yang unik (polymorphism)
                    if (report is MaintenanceReport maintenanceReport)  // <-- INHERITANCE: Type checking untuk child class
                    {
                        // POLYMORPHISM: IsUrgent() hanya ada di MaintenanceReport
                        // Method ini tidak ada di BaseReport atau child class lainnya
                        if (maintenanceReport.IsUrgent())  // <-- POLYMORPHISM: Method khusus dari MaintenanceReport
                        {
                            urgentReports.Add($"Maintenance: {maintenanceReport.EquipmentName} - {maintenanceReport.Priority}");
                        }
                    }
                    else if (report is SafetyReport safetyReport)  // <-- INHERITANCE: Type checking untuk child class
                    {
                        // POLYMORPHISM: RequiresEmergencyResponse() hanya ada di SafetyReport
                        // Method ini tidak ada di BaseReport atau child class lainnya
                        if (safetyReport.RequiresEmergencyResponse())  // <-- POLYMORPHISM: Method khusus dari SafetyReport
                        {
                            emergencyReports.Add($"Safety: {safetyReport.IncidentType} - {safetyReport.Severity}");
                        }
                    }
                    else if (report is WeatherReport weatherReport)  // <-- INHERITANCE: Type checking untuk child class
                    {
                        // POLYMORPHISM: IsSevereWeather() hanya ada di WeatherReport
                        // Method ini tidak ada di BaseReport atau child class lainnya
                        if (weatherReport.IsSevereWeather())  // <-- POLYMORPHISM: Method khusus dari WeatherReport
                        {
                            severeWeatherReports.Add($"Weather: {weatherReport.Category} - {weatherReport.Temperature}°C");
                        }
                    }

                    // ===========================================
                    // POLYMORPHISM: Runtime Polymorphism - Method Overriding
                    // ===========================================
                    // GetSummary() dipanggil sesuai override-nya
                    // - SafetyReport.GetSummary() -> implementasi SafetyReport
                    // - MaintenanceReport.GetSummary() -> implementasi MaintenanceReport
                    // - WeatherReport.GetSummary() -> implementasi WeatherReport
                    string summary = report.GetSummary();  // <-- POLYMORPHISM: Method dipanggil sesuai tipe sebenarnya (runtime binding)
                    allReportDetails.Add($"Summary: {summary}");
                }
                else
                {
                    invalidCount++;
                    allReportDetails.Add($"Invalid Report: {report.GetType().Name} at {report.Location}");
                }
            }

            // ===========================================
            // ENCAPSULATION: Memanggil private method
            // ===========================================
            // CalculateStatistics() adalah private method yang hanya bisa diakses dari dalam class ini
            // Ini adalah contoh encapsulation - data internal dihitung oleh private method
            var statistics = CalculateStatistics(processedCount, validCount, invalidCount);  // <-- ENCAPSULATION: Memanggil private method

            // Return hasil dengan semua informasi
            return new Dictionary<string, object>
            {
                { "TotalProcessed", processedCount },
                { "ValidReports", validCount },
                { "InvalidReports", invalidCount },
                { "Statistics", statistics },
                { "UrgentMaintenance", urgentReports },
                { "EmergencySafety", emergencyReports },
                { "SevereWeather", severeWeatherReports },
                { "AllDetails", allReportDetails }
            };
        }

        /// <summary>
        /// ===========================================
        /// ENCAPSULATION: Private method
        /// ===========================================
        /// Method ini hanya bisa diakses dari dalam class ReportService
        /// Tidak bisa diakses dari luar class (data hiding)
        /// </summary>
        private Dictionary<string, object> CalculateStatistics(int total, int valid, int invalid)  // <-- ENCAPSULATION: private keyword menunjukkan method ini hanya bisa diakses dari dalam class
        {
            // ENCAPSULATION: Local variables dalam private method
            var validPercentage = total > 0 ? (double)valid / total * 100 : 0;
            var invalidPercentage = total > 0 ? (double)invalid / total * 100 : 0;

            return new Dictionary<string, object>
            {
                { "ValidPercentage", $"{validPercentage:F2}%" },
                { "InvalidPercentage", $"{invalidPercentage:F2}%" },
                { "SuccessRate", validPercentage >= 80 ? "Good" : "Needs Attention" }
            };
        }

        /// <summary>
        /// ===========================================
        /// INHERITANCE & POLYMORPHISM: Method untuk membuat sample reports
        /// ===========================================
        /// Method ini menunjukkan Inheritance dan Polymorphism
        /// </summary>
        public List<BaseReport> CreateSampleReports()  // <-- INHERITANCE & POLYMORPHISM: Return type BaseReport tapi bisa berisi child classes
        {
            var reports = new List<BaseReport>();  // <-- INHERITANCE & POLYMORPHISM: List bertipe BaseReport tapi bisa menyimpan child classes

            // ===========================================
            // INHERITANCE: Membuat instance dari child classes
            // POLYMORPHISM: Disimpan sebagai BaseReport tapi behavior-nya sesuai child class
            // ===========================================

            // INHERITANCE: SafetyReport adalah child class dari BaseReport
            var safetyReport = new SafetyReport  // <-- INHERITANCE: SafetyReport inherit dari BaseReport
            {
                ReportID = 1,  // <-- INHERITANCE: Property diwarisi dari BaseReport
                Location = "Java Sea",  // <-- INHERITANCE: Property diwarisi dari BaseReport
                IncidentType = "Fire",  // <-- Property khusus SafetyReport
                Severity = "Critical",  // <-- Property khusus SafetyReport
                Description = "Engine room fire detected",  // <-- Property khusus SafetyReport
                UserID = 1,
                CreatedAt = DateTime.Now  // <-- INHERITANCE: Property diwarisi dari BaseReport
            };
            reports.Add(safetyReport);  // <-- POLYMORPHISM: SafetyReport disimpan sebagai BaseReport

            // INHERITANCE: MaintenanceReport adalah child class dari BaseReport
            var maintenanceReport = new MaintenanceReport  // <-- INHERITANCE: MaintenanceReport inherit dari BaseReport
            {
                ReportID = 2,  // <-- INHERITANCE: Property diwarisi dari BaseReport
                Location = "Surabaya Port",  // <-- INHERITANCE: Property diwarisi dari BaseReport
                EquipmentName = "Main Engine",  // <-- Property khusus MaintenanceReport
                IssueDescription = "Overheating issue",  // <-- Property khusus MaintenanceReport
                Priority = "Urgent",  // <-- Property khusus MaintenanceReport
                ShipID = 1,
                UserID = 1,
                CreatedAt = DateTime.Now  // <-- INHERITANCE: Property diwarisi dari BaseReport
            };
            reports.Add(maintenanceReport);  // <-- POLYMORPHISM: MaintenanceReport disimpan sebagai BaseReport

            // INHERITANCE: WeatherReport adalah child class dari BaseReport
            var weatherReport = new WeatherReport  // <-- INHERITANCE: WeatherReport inherit dari BaseReport
            {
                ReportID = 3,  // <-- INHERITANCE: Property diwarisi dari BaseReport
                Location = "South China Sea",  // <-- INHERITANCE: Property diwarisi dari BaseReport
                Reporter = "Captain John",  // <-- Property khusus WeatherReport
                Category = "Storm",  // <-- Property khusus WeatherReport
                Severity = "High",  // <-- Property khusus WeatherReport
                Temperature = 28.5,  // <-- Property khusus WeatherReport
                WindSpeed = "45 knots",  // <-- Property khusus WeatherReport
                SeaCondition = "Rough",  // <-- Property khusus WeatherReport
                Description = "Heavy storm approaching",  // <-- Property khusus WeatherReport
                UserID = 1,
                CreatedAt = DateTime.Now  // <-- INHERITANCE: Property diwarisi dari BaseReport
            };
            reports.Add(weatherReport);  // <-- POLYMORPHISM: WeatherReport disimpan sebagai BaseReport

            // INHERITANCE: MaintenanceReport adalah child class dari BaseReport
            var normalMaintenance = new MaintenanceReport  // <-- INHERITANCE: MaintenanceReport inherit dari BaseReport
            {
                ReportID = 4,  // <-- INHERITANCE: Property diwarisi dari BaseReport
                Location = "Jakarta Port",  // <-- INHERITANCE: Property diwarisi dari BaseReport
                EquipmentName = "Navigation System",  // <-- Property khusus MaintenanceReport
                IssueDescription = "Routine maintenance",  // <-- Property khusus MaintenanceReport
                Priority = "Normal",  // <-- Property khusus MaintenanceReport
                ShipID = 2,
                UserID = 2,
                CreatedAt = DateTime.Now  // <-- INHERITANCE: Property diwarisi dari BaseReport
            };
            reports.Add(normalMaintenance);  // <-- POLYMORPHISM: MaintenanceReport disimpan sebagai BaseReport

            return reports;  // <-- POLYMORPHISM: List berisi berbagai child classes yang di-treat sebagai BaseReport
        }
    }
}

