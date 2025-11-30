namespace Marinex.Models
{

    public class SafetyReport : BaseReport
    {
        public string IncidentType { get; set; }
        public string Severity { get; set; }
        public string Description { get; set; }
        public int UserID { get; set; }
        
        public override string GenerateReport()  {
            return $"SAFETY REPORT\n" +
                   $"Location: {Location}\n" + 
                   $"Incident Type: {IncidentType}\n" +
                   $"Severity: {Severity}\n" +
                   $"Description: {Description}\n" +
                   $"Reported on: {CreatedAt:yyyy-MM-dd HH:mm:ss}";  
        }
        
        public override string GetSummary()  
        {
            return base.GetSummary() + $" - {IncidentType} ({Severity})";  
        }
        
        public bool RequiresEmergencyResponse()
        {
            return Severity?.ToLower() == "critical" || Severity?.ToLower() == "high";
        }
        
    }
}

