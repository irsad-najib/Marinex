namespace Marinex.Models
{
    /// <summary>
    /// ===========================================
    /// INHERITANCE: Child class dari BaseReport
    /// ===========================================
    /// WeatherReport INHERIT dari BaseReport menggunakan keyword ":"
    /// Ini adalah contoh Inheritance - WeatherReport mewarisi semua property dan method dari BaseReport
    /// </summary>
    public class WeatherReport : BaseReport  // <-- INHERITANCE: Syntax ": BaseReport" menunjukkan inheritance
    {
        // ===========================================
        // ENCAPSULATION: Properties khusus untuk Weather Report
        // ===========================================
        // Properties ini mengenkapsulasi data khusus untuk WeatherReport
        public string Reporter { get; set; }
        public string Category { get; set; }
        public string Severity { get; set; }
        public string Description { get; set; }
        public double Temperature { get; set; }
        public string WindSpeed { get; set; }
        public string SeaCondition { get; set; }

        // Relasi
        public int UserID { get; set; }
        public User User { get; set; }

        // ===========================================
        // POLYMORPHISM: Method Overriding
        // ===========================================
        // Override abstract method GenerateReport() dari BaseReport
        // Implementasi ini BERBEDA dengan SafetyReport dan MaintenanceReport (POLYMORPHISM)
        public override string GenerateReport()  // <-- POLYMORPHISM
        {
            return $"WEATHER REPORT\n" +
                   $"Location: {Location}\n" +  
                   $"Reporter: {Reporter}\n" +
                   $"Category: {Category}\n" +
                   $"Severity: {Severity}\n" +
                   $"Temperature: {Temperature}°C\n" +
                   $"Wind Speed: {WindSpeed}\n" +
                   $"Sea Condition: {SeaCondition}\n" +
                   $"Description: {Description}\n" +
                   $"Reported on: {CreatedAt:yyyy-MM-dd HH:mm:ss}"; 
        }

        // ===========================================
        // POLYMORPHISM: Method Overriding
        // ===========================================
        // Override virtual method GetSummary() dari BaseReport
        // Implementasi ini BERBEDA dengan SafetyReport dan MaintenanceReport (POLYMORPHISM)
        public override string GetSummary()  // <-- POLYMORPHISM: override keyword menunjukkan method overriding
        {
            return base.GetSummary() + $" - {Category} ({Severity}) - {Temperature}°C";  // <-- INHERITANCE: base.GetSummary() memanggil method dari parent class
        }

        // ===========================================
        // POLYMORPHISM: Method Overriding dengan validasi khusus
        // ===========================================
        // Override virtual method Validate() dari BaseReport
        // Implementasi ini BERBEDA dengan BaseReport (menambahkan validasi tambahan)
        public override bool Validate()  // <-- POLYMORPHISM: override keyword menunjukkan method overriding
        {
            return base.Validate() &&  // <-- INHERITANCE: base.Validate() memanggil method dari parent class
                   !string.IsNullOrEmpty(Category) && 
                   !string.IsNullOrEmpty(Severity) &&
                   !string.IsNullOrEmpty(Reporter);
        }

        // ===========================================
        // Method khusus untuk Weather Report
        // ===========================================
        // Method ini hanya ada di WeatherReport, tidak ada di BaseReport atau child class lainnya
        public bool IsSevereWeather()
        {
            return Severity?.ToLower() == "critical" || 
                   Severity?.ToLower() == "high" ||
                   (Temperature < 0 || Temperature > 40);
        }
    }
}
