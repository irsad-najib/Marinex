using System;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Marinex.Services;
using Marinex.Models;

namespace Marinex.Views
{
    public partial class DashboardView : UserControl
    {
        private ReportService _reportService;
        private AccessModifierDemoService _accessModifierDemo;

        public DashboardView()
        {
            InitializeComponent();
            _reportService = new ReportService();
            _accessModifierDemo = new AccessModifierDemoService();
        }

        private void BtnDemonstrateOOP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var output = new StringBuilder();
                output.AppendLine("╔══════════════════════════════════════════════════════════════╗");
                output.AppendLine("║  DEMONSTRASI OOP: INHERITANCE, ENCAPSULATION, POLYMORPHISM  ║");
                output.AppendLine("╚══════════════════════════════════════════════════════════════╝");
                output.AppendLine();

                // 1. INHERITANCE: Membuat sample reports dari child classes
                output.AppendLine("┌─────────────────────────────────────────────────────────────┐");
                output.AppendLine("│ 1. INHERITANCE: Membuat instance dari child classes        │");
                output.AppendLine("│    (SafetyReport, MaintenanceReport, WeatherReport)        │");
                output.AppendLine("│    Semua inherit dari BaseReport                          │");
                output.AppendLine("└─────────────────────────────────────────────────────────────┘");
                output.AppendLine();

                var sampleReports = _reportService.CreateSampleReports();
                output.AppendLine($"✓ Berhasil membuat {sampleReports.Count} sample reports:");
                foreach (var report in sampleReports)
                {
                    output.AppendLine($"  - {report.GetType().Name} at {report.Location}");
                }
                output.AppendLine();

                // 2. POLYMORPHISM: Memproses reports dengan method yang menerima BaseReport
                output.AppendLine("┌─────────────────────────────────────────────────────────────┐");
                output.AppendLine("│ 2. POLYMORPHISM: Method ProcessReportsWithOOPConcepts()     │");
                output.AppendLine("│    Menerima List<BaseReport> tapi behavior berbeda          │");
                output.AppendLine("│    sesuai tipe sebenarnya (runtime polymorphism)           │");
                output.AppendLine("└─────────────────────────────────────────────────────────────┘");
                output.AppendLine();

                var results = _reportService.ProcessReportsWithOOPConcepts(sampleReports);

                // 3. ENCAPSULATION: Menampilkan hasil dari private fields dan methods
                output.AppendLine("┌─────────────────────────────────────────────────────────────┐");
                output.AppendLine("│ 3. ENCAPSULATION: Private fields dan methods di dalam class │");
                output.AppendLine("│    Data internal tidak langsung diakses dari luar           │");
                output.AppendLine("└─────────────────────────────────────────────────────────────┘");
                output.AppendLine();

                output.AppendLine("📊 STATISTIK PEMROSESAN:");
                output.AppendLine($"   Total Reports: {results["TotalProcessed"]}");
                output.AppendLine($"   Valid Reports: {results["ValidReports"]}");
                output.AppendLine($"   Invalid Reports: {results["InvalidReports"]}");

                var statistics = (Dictionary<string, object>)results["Statistics"];
                output.AppendLine($"   Valid Percentage: {statistics["ValidPercentage"]}");
                output.AppendLine($"   Invalid Percentage: {statistics["InvalidPercentage"]}");
                output.AppendLine($"   Success Rate: {statistics["SuccessRate"]}");
                output.AppendLine();

                // 4. POLYMORPHISM: Menampilkan method khusus dari tiap child class
                output.AppendLine("┌─────────────────────────────────────────────────────────────┐");
                output.AppendLine("│ 4. POLYMORPHISM: Method khusus dari tiap child class        │");
                output.AppendLine("│    - MaintenanceReport.IsUrgent()                           │");
                output.AppendLine("│    - SafetyReport.RequiresEmergencyResponse()                │");
                output.AppendLine("│    - WeatherReport.IsSevereWeather()                       │");
                output.AppendLine("└─────────────────────────────────────────────────────────────┘");
                output.AppendLine();

                var urgentMaintenance = (List<string>)results["UrgentMaintenance"];
                if (urgentMaintenance.Count > 0)
                {
                    output.AppendLine("⚠️  URGENT MAINTENANCE REPORTS:");
                    foreach (var item in urgentMaintenance)
                    {
                        output.AppendLine($"   - {item}");
                    }
                    output.AppendLine();
                }

                var emergencySafety = (List<string>)results["EmergencySafety"];
                if (emergencySafety.Count > 0)
                {
                    output.AppendLine("🚨 EMERGENCY SAFETY REPORTS:");
                    foreach (var item in emergencySafety)
                    {
                        output.AppendLine($"   - {item}");
                    }
                    output.AppendLine();
                }

                var severeWeather = (List<string>)results["SevereWeather"];
                if (severeWeather.Count > 0)
                {
                    output.AppendLine("🌪️  SEVERE WEATHER REPORTS:");
                    foreach (var item in severeWeather)
                    {
                        output.AppendLine($"   - {item}");
                    }
                    output.AppendLine();
                }

                // 5. POLYMORPHISM: Menampilkan output dari GenerateReport() yang berbeda
                output.AppendLine("┌─────────────────────────────────────────────────────────────┐");
                output.AppendLine("│ 5. POLYMORPHISM: GenerateReport() dengan implementasi       │");
                output.AppendLine("│    berbeda untuk tiap child class (method overriding)       │");
                output.AppendLine("└─────────────────────────────────────────────────────────────┘");
                output.AppendLine();

                var allDetails = (List<string>)results["AllDetails"];
                foreach (var detail in allDetails)
                {
                    output.AppendLine(detail);
                    output.AppendLine();
                }

                // 6. Kesimpulan
                output.AppendLine("┌─────────────────────────────────────────────────────────────┐");
                output.AppendLine("│ ✅ KESIMPULAN:                                                │");
                output.AppendLine("│                                                                │");
                output.AppendLine("│ 1. INHERITANCE:                                              │");
                output.AppendLine("│    ✓ SafetyReport, MaintenanceReport, WeatherReport         │");
                output.AppendLine("│      inherit dari BaseReport                                │");
                output.AppendLine("│                                                                │");
                output.AppendLine("│ 2. ENCAPSULATION:                                            │");
                output.AppendLine("│    ✓ Private fields dan methods di ReportService            │");
                output.AppendLine("│    ✓ Data internal tidak langsung diakses dari luar         │");
                output.AppendLine("│                                                                │");
                output.AppendLine("│ 3. POLYMORPHISM:                                             │");
                output.AppendLine("│    ✓ Method ProcessReportsWithOOPConcepts() menerima         │");
                output.AppendLine("│      BaseReport tapi behavior sesuai tipe sebenarnya        │");
                output.AppendLine("│    ✓ GenerateReport() dipanggil sesuai implementasi         │");
                output.AppendLine("│      masing-masing child class                              │");
                output.AppendLine("│    ✓ Method khusus (IsUrgent(), RequiresEmergencyResponse(),│");
                output.AppendLine("│      IsSevereWeather()) dipanggil sesuai tipe report        │");
                output.AppendLine("└─────────────────────────────────────────────────────────────┘");

                txtOOPOutput.Text = output.ToString();
            }
            catch (Exception ex)
            {
                txtOOPOutput.Text = $"Error: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}";
            }
        }

        private void BtnDemonstrateProtected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var output = new StringBuilder();
                output.AppendLine("╔══════════════════════════════════════════════════════════════╗");
                output.AppendLine("║  DEMONSTRASI ACCESS MODIFIER: PROTECTED                      ║");
                output.AppendLine("╚══════════════════════════════════════════════════════════════╝");
                output.AppendLine();

                output.AppendLine("┌─────────────────────────────────────────────────────────────┐");
                output.AppendLine("│ ACCESS MODIFIER: PROTECTED                                  │");
                output.AppendLine("│ Protected members bisa diakses dari:                        │");
                output.AppendLine("│ - Class yang sama                                            │");
                output.AppendLine("│ - Derived classes (child classes)                           │");
                output.AppendLine("│ Protected members TIDAK bisa diakses dari luar class hierarchy│");
                output.AppendLine("└─────────────────────────────────────────────────────────────┘");
                output.AppendLine();

                // Demonstrasi protected access
                var safetyReport = new SafetyReport
                {
                    ReportID = 1,
                    Location = "Java Sea",
                    IncidentType = "Fire",
                    Severity = "Critical",
                    Description = "Engine room fire",
                    UserID = 1,
                    CreatedAt = DateTime.Now
                };

                // Menggunakan protected melalui public property
                output.AppendLine("1. Mengakses PROTECTED field melalui PUBLIC property:");
                safetyReport.ReportStatus = "Active";  // <-- PUBLIC property mengakses PROTECTED field
                safetyReport.Version = 1;  // <-- PUBLIC property mengakses PROTECTED field
                output.AppendLine($"   Report Status: {safetyReport.ReportStatus}");  // <-- Mengakses protected via public
                output.AppendLine($"   Version: {safetyReport.Version}");  // <-- Mengakses protected via public
                output.AppendLine();

                // Menggunakan protected method dari child class
                output.AppendLine("2. Mengakses PROTECTED members dari child class:");
                safetyReport.InitializeReport();  // <-- Method ini mengakses protected fields dan methods
                output.AppendLine($"   Report Status (after InitializeReport): {safetyReport.ReportStatus}");
                output.AppendLine($"   Protected Info: {safetyReport.GetProtectedInfo()}");
                output.AppendLine();

                output.AppendLine("┌─────────────────────────────────────────────────────────────┐");
                output.AppendLine("│ BUKTI PROTECTED ACCESS MODIFIER:                            │");
                output.AppendLine("│                                                               │");
                output.AppendLine("│ Di BaseReport.cs:                                            │");
                output.AppendLine("│ - protected string _reportStatus;  // PROTECTED field        │");
                output.AppendLine("│ - protected int _version;  // PROTECTED field                │");
                output.AppendLine("│ - protected virtual string GetReportStatus();  // PROTECTED   │");
                output.AppendLine("│ - protected void SetReportStatus(string);  // PROTECTED       │");
                output.AppendLine("│                                                               │");
                output.AppendLine("│ Di SafetyReport.cs:                                          │");
                output.AppendLine("│ - InitializeReport() mengakses _reportStatus dan _version   │");
                output.AppendLine("│   (protected fields dari parent class)                      │");
                output.AppendLine("│ - GetProtectedInfo() memanggil GetReportStatus()            │");
                output.AppendLine("│   (protected method dari parent class)                      │");
                output.AppendLine("└─────────────────────────────────────────────────────────────┘");

                var demoResult = _accessModifierDemo.DemonstrateProtectedAccess(safetyReport);
                output.AppendLine();
                output.AppendLine(demoResult);

                txtAccessModifierOutput.Text = output.ToString();
            }
            catch (Exception ex)
            {
                txtAccessModifierOutput.Text = $"Error: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}";
            }
        }

        private void BtnDemonstrateInternal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var output = new StringBuilder();
                output.AppendLine("╔══════════════════════════════════════════════════════════════╗");
                output.AppendLine("║  DEMONSTRASI ACCESS MODIFIER: INTERNAL                      ║");
                output.AppendLine("╚══════════════════════════════════════════════════════════════╝");
                output.AppendLine();

                output.AppendLine("┌─────────────────────────────────────────────────────────────┐");
                output.AppendLine("│ ACCESS MODIFIER: INTERNAL                                   │");
                output.AppendLine("│ Internal members bisa diakses dari:                         │");
                output.AppendLine("│ - Class lain dalam assembly yang sama (Marinex)             │");
                output.AppendLine("│ Internal members TIDAK bisa diakses dari assembly lain      │");
                output.AppendLine("└─────────────────────────────────────────────────────────────┘");
                output.AppendLine();

                var demoResult = _accessModifierDemo.DemonstrateInternalAccess();
                output.AppendLine(demoResult);
                output.AppendLine();

                output.AppendLine("┌─────────────────────────────────────────────────────────────┐");
                output.AppendLine("│ BUKTI INTERNAL ACCESS MODIFIER:                            │");
                output.AppendLine("│                                                               │");
                output.AppendLine("│ Di ReportService.cs:                                         │");
                output.AppendLine("│ - internal int GetTotalProcessedReports();  // INTERNAL      │");
                output.AppendLine("│ - internal void IncrementProcessedReports();  // INTERNAL   │");
                output.AppendLine("│ - internal static int GetInstanceCount();  // INTERNAL    │");
                output.AppendLine("│                                                               │");
                output.AppendLine("│ Di AccessModifierDemo.cs:                                   │");
                output.AppendLine("│ - internal class AccessModifierDemo  // INTERNAL class     │");
                output.AppendLine("│ - internal string InternalData  // INTERNAL property        │");
                output.AppendLine("│ - internal string GetInternalData();  // INTERNAL method     │");
                output.AppendLine("│                                                               │");
                output.AppendLine("│ Di AccessModifierDemoService.cs:                            │");
                output.AppendLine("│ - Bisa mengakses internal class AccessModifierDemo          │");
                output.AppendLine("│ - Bisa memanggil internal methods dari ReportService        │");
                output.AppendLine("│   karena dalam assembly yang sama (Marinex)                  │");
                output.AppendLine("└─────────────────────────────────────────────────────────────┘");

                txtAccessModifierOutput.Text = output.ToString();
            }
            catch (Exception ex)
            {
                txtAccessModifierOutput.Text = $"Error: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}";
            }
        }
    }
}
