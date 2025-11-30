namespace Marinex.Models
{
    public class WeatherReport : BaseReport  
    {
        public string Reporter { get; set; }
        public string Category { get; set; }
        public string Severity { get; set; }
        public string Description { get; set; }
        public double Temperature { get; set; }
        public string WindSpeed { get; set; }
        public string SeaCondition { get; set; }

        // Relasi
        public int UserID { get; set; }
        
        public override string GenerateReport()  
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

        public override string GetSummary() 
        {
            return base.GetSummary() + $" - {Category} ({Severity}) - {Temperature}°C";  
        }

        
        public override bool Validate()  
        {
            return base.Validate() &&  
                   !string.IsNullOrEmpty(Category) && 
                   !string.IsNullOrEmpty(Severity) &&
                   !string.IsNullOrEmpty(Reporter);
        }

        public bool IsSevereWeather()
        {
            return Severity?.ToLower() == "critical" || 
                   Severity?.ToLower() == "high" ||
                   (Temperature < 0 || Temperature > 40);
        }
    }
}
