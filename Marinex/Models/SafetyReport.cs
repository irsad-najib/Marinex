namespace Marinex.Models
{
    /// <summary>
    /// Child class yang inherit dari BaseReport - Demonstrasi Inheritance
    /// Implementasi khusus untuk Safety Report
    /// </summary>
    public class SafetyReport : BaseReport
    {
        // Encapsulation: Properties khusus untuk Safety Report
        public string IncidentType { get; set; }
        public string Severity { get; set; }
        public string Description { get; set; }
        public int UserID { get; set; }
        
        // Polymorphism: Override abstract method dari base class
        public override string GenerateReport()
        {
            return $"SAFETY REPORT\n" +
                   $"Location: {Location}\n" +
                   $"Incident Type: {IncidentType}\n" +
                   $"Severity: {Severity}\n" +
                   $"Description: {Description}\n" +
                   $"Reported on: {CreatedAt:yyyy-MM-dd HH:mm:ss}";
        }
        
        // Polymorphism: Override virtual method dari base class
        public override string GetSummary()
        {
            return base.GetSummary() + $" - {IncidentType} ({Severity})";
        }
        
        // Method khusus untuk Safety Report
        public bool RequiresEmergencyResponse()
        {
            return Severity?.ToLower() == "critical" || Severity?.ToLower() == "high";
        }
    }
}

