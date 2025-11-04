using Marinex.Models;

namespace Marinex.Services
{
    /// <summary>
    /// Service untuk demonstrasi Inheritance dan Polymorphism
    /// Menggunakan BaseReport sebagai base class dan menerapkan Polymorphism
    /// </summary>
    public class ReportService
    {
        /// <summary>
        /// Demonstrasi Polymorphism - method yang menerima base class
        /// tapi bisa bekerja dengan berbagai child class (SafetyReport, MaintenanceReport)
        /// </summary>
        public string ProcessReport(BaseReport report)
        {
            // Polymorphism: Method ini bisa menerima SafetyReport atau MaintenanceReport
            // karena mereka inherit dari BaseReport
            if (!report.Validate())
            {
                return "Report validation failed";
            }

            // Polymorphism: GenerateReport() akan dipanggil sesuai dengan tipe sebenarnya
            // SafetyReport.GenerateReport() atau MaintenanceReport.GenerateReport()
            return report.GenerateReport();
        }

        /// <summary>
        /// Demonstrasi Polymorphism - method yang bisa bekerja dengan berbagai implementasi
        /// </summary>
        public string GetReportSummary(BaseReport report)
        {
            // Polymorphism: GetSummary() akan dipanggil sesuai dengan override-nya
            return report.GetSummary();
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
        /// Demonstrasi Polymorphism dengan collection
        /// Bisa menerima berbagai jenis report dan memprosesnya secara polymorphic
        /// </summary>
        public List<string> ProcessMultipleReports(List<BaseReport> reports)
        {
            var results = new List<string>();
            
            // Polymorphism: Loop melalui berbagai jenis report
            foreach (var report in reports)
            {
                // Polymorphism: GenerateReport() akan dipanggil sesuai tipe sebenarnya
                results.Add(report.GenerateReport());
            }
            
            return results;
        }
    }
}

