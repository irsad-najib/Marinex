namespace Marinex.Models
{
    /// <summary>
    /// ===========================================
    /// INHERITANCE: Child class dari BaseReport
    /// ===========================================
    /// SafetyReport INHERIT dari BaseReport menggunakan keyword ":"
    /// Ini adalah contoh Inheritance - SafetyReport mewarisi semua property dan method dari BaseReport
    /// </summary>
    public class SafetyReport : BaseReport  // <-- INHERITANCE: Syntax ": BaseReport" menunjukkan inheritance
    {
        // ===========================================
        // ENCAPSULATION: Properties khusus untuk Safety Report
        // ===========================================
        // Properties ini mengenkapsulasi data khusus untuk SafetyReport
        public string IncidentType { get; set; }
        public string Severity { get; set; }
        public string Description { get; set; }
        public int UserID { get; set; }
        
        // ===========================================
        // POLYMORPHISM: Method Overriding
        // ===========================================
        // Override abstract method GenerateReport() dari BaseReport
        // Implementasi ini BERBEDA dengan MaintenanceReport dan WeatherReport (POLYMORPHISM)
        public override string GenerateReport()  // <-- POLYMORPHISM: override keyword menunjukkan method overriding
        {
            return $"SAFETY REPORT\n" +
                   $"Location: {Location}\n" +  // <-- INHERITANCE: Location diwarisi dari BaseReport
                   $"Incident Type: {IncidentType}\n" +
                   $"Severity: {Severity}\n" +
                   $"Description: {Description}\n" +
                   $"Reported on: {CreatedAt:yyyy-MM-dd HH:mm:ss}";  // <-- INHERITANCE: CreatedAt diwarisi dari BaseReport
        }
        
        // ===========================================
        // POLYMORPHISM: Method Overriding
        // ===========================================
        // Override virtual method GetSummary() dari BaseReport
        // Implementasi ini BERBEDA dengan child class lainnya (POLYMORPHISM)
        public override string GetSummary()  // <-- POLYMORPHISM: override keyword menunjukkan method overriding
        {
            return base.GetSummary() + $" - {IncidentType} ({Severity})";  // <-- INHERITANCE: base.GetSummary() memanggil method dari parent class
        }
        
        // ===========================================
        // ACCESS MODIFIER: PROTECTED - Mengakses protected field dari parent class
        // ===========================================
        // SafetyReport bisa mengakses protected field _reportStatus dan _version dari BaseReport
        // Ini menunjukkan bahwa protected members bisa diakses dari derived classes
        public void InitializeReport()
        {
            // PROTECTED: Mengakses protected field dari parent class (BaseReport)
            _reportStatus = "Initialized";  // <-- ACCESS MODIFIER: protected - bisa diakses dari child class
            _version = 1;  // <-- ACCESS MODIFIER: protected - bisa diakses dari child class
            
            // PROTECTED: Memanggil protected method dari parent class
            SetReportStatus("Active");  // <-- ACCESS MODIFIER: protected - bisa dipanggil dari child class
        }
        
        // ===========================================
        // Method khusus untuk Safety Report
        // ===========================================
        // Method ini hanya ada di SafetyReport, tidak ada di BaseReport atau child class lainnya
        public bool RequiresEmergencyResponse()
        {
            return Severity?.ToLower() == "critical" || Severity?.ToLower() == "high";
        }
        
        // ===========================================
        // ACCESS MODIFIER: PUBLIC - Method untuk menunjukkan protected access
        // ===========================================
        public string GetProtectedInfo()
        {
            // PROTECTED: Mengakses protected field dan method dari parent class
            return $"Report Status: {GetReportStatus()}, Version: {_version}";  // <-- Mengakses protected member
        }
    }
}

