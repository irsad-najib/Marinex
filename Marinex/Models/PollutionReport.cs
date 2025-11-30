namespace Marinex.Models
{
    public class PollutionReport : BaseReport
    {
        public string WasteType { get; set; }
        public string Quantity { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string PhotoPaths { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
        public string EnvironmentalImpact { get; set; }
        public string ActionTaken { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
        
        public int? ShipID { get; set; }
        
        public override string GenerateReport()
        {
            return $"🗑️ POLLUTION/WASTE REPORT\n" +
                   $"{new string('=', 50)}\n" +
                   $"Location: {Location}\n" +
                   $"Coordinates: {Latitude:F6}, {Longitude:F6}\n" +
                   $"Waste Type: {WasteType}\n" +
                   $"Quantity: {Quantity}\n" +
                   $"Severity: {Severity}\n" +
                   $"Status: {Status}\n" +
                   $"Description:\n{Description}\n" +
                   $"Environmental Impact: {EnvironmentalImpact ?? "Not assessed"}\n" +
                   $"Action Taken: {ActionTaken ?? "None yet"}\n" +
                   $"Reported on: {CreatedAt:yyyy-MM-dd HH:mm:ss}\n" +
                   $"Photos: {(string.IsNullOrEmpty(PhotoPaths) ? "No photos" : "Available")}\n" +
                   $"{new string('=', 50)}\n";
        }
        
        public override bool Validate()
        {
            return base.Validate() &&
                   !string.IsNullOrEmpty(WasteType) &&
                   !string.IsNullOrEmpty(Quantity) &&
                   !string.IsNullOrEmpty(Severity) &&
                   Latitude >= -90 && Latitude <= 90 &&
                   Longitude >= -180 && Longitude <= 180;
        }
        
    }
}
