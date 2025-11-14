namespace Marinex.Models
{
    /// <summary>
    /// ===========================================
    /// INHERITANCE: Child class dari BaseReport
    /// ===========================================
    /// MaintenanceReport INHERIT dari BaseReport menggunakan keyword ":"
    /// Ini adalah contoh Inheritance - MaintenanceReport mewarisi semua property dan method dari BaseReport
    /// </summary>
    public class MaintenanceReport : BaseReport  // <-- INHERITANCE: Syntax ": BaseReport" menunjukkan inheritance
    {
        // ===========================================
        // ENCAPSULATION: Properties khusus untuk Maintenance Report
        // ===========================================
        // Properties ini mengenkapsulasi data khusus untuk MaintenanceReport
        public string EquipmentName { get; set; }
        public string IssueDescription { get; set; }
        public string Priority { get; set; }
        public int ShipID { get; set; }
        public int UserID { get; set; }
        
        // ===========================================
        // POLYMORPHISM: Method Overriding
        // ===========================================
        // Override abstract method GenerateReport() dari BaseReport
        // Implementasi ini BERBEDA dengan SafetyReport dan WeatherReport (POLYMORPHISM)
        // Meskipun method signature sama, behavior-nya berbeda (runtime polymorphism)
        public override string GenerateReport()  // <-- POLYMORPHISM: override keyword menunjukkan method overriding
        {
            return $"MAINTENANCE REPORT\n" +
                   $"Location: {Location}\n" +  // <-- INHERITANCE: Location diwarisi dari BaseReport
                   $"Equipment: {EquipmentName}\n" +
                   $"Issue: {IssueDescription}\n" +
                   $"Priority: {Priority}\n" +
                   $"Reported on: {CreatedAt:yyyy-MM-dd HH:mm:ss}";  // <-- INHERITANCE: CreatedAt diwarisi dari BaseReport
        }
        
        // ===========================================
        // POLYMORPHISM: Method Overriding
        // ===========================================
        // Override virtual method GetSummary() dari BaseReport
        // Implementasi ini BERBEDA dengan SafetyReport dan WeatherReport (POLYMORPHISM)
        public override string GetSummary()  // <-- POLYMORPHISM: override keyword menunjukkan method overriding
        {
            return base.GetSummary() + $" - Equipment: {EquipmentName} (Priority: {Priority})";  // <-- INHERITANCE: base.GetSummary() memanggil method dari parent class
        }
        
        // ===========================================
        // Method khusus untuk Maintenance Report
        // ===========================================
        // Method ini hanya ada di MaintenanceReport, tidak ada di BaseReport atau child class lainnya
        public bool IsUrgent()
        {
            return Priority?.ToLower() == "urgent" || Priority?.ToLower() == "high";
        }
    }
}

