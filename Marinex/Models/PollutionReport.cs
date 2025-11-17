namespace Marinex.Models
{
    /// <summary>
    /// ===========================================
    /// INHERITANCE: Child class dari BaseReport
    /// ===========================================
    /// PollutionReport INHERIT dari BaseReport untuk pelaporan sampah/polusi di laut
    /// Mendukung tracking waste type, quantity, coordinates, dan photos
    /// </summary>
    public class PollutionReport : BaseReport
    {
        // ===========================================
        // ENCAPSULATION: Properties khusus untuk Pollution Report
        // ===========================================
        
        /// <summary>
        /// Jenis sampah: Plastic, Oil Spill, Chemical, Marine Debris, etc.
        /// </summary>
        public string WasteType { get; set; }
        
        /// <summary>
        /// Estimasi quantity (e.g., "Small", "Medium", "Large", "Massive")
        /// </summary>
        public string Quantity { get; set; }
        
        /// <summary>
        /// Deskripsi detail polusi/sampah yang ditemukan
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Koordinat latitude lokasi sampah ditemukan
        /// </summary>
        public double Latitude { get; set; }
        
        /// <summary>
        /// Koordinat longitude lokasi sampah ditemukan
        /// </summary>
        public double Longitude { get; set; }
        
        /// <summary>
        /// Path/URL foto bukti (bisa multiple, dipisah dengan semicolon)
        /// </summary>
        public string PhotoPaths { get; set; }
        
        /// <summary>
        /// Severity level: Low, Medium, High, Critical
        /// </summary>
        public string Severity { get; set; }
        
        /// <summary>
        /// Status: Reported, Under Investigation, Cleanup Initiated, Resolved
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// Estimasi dampak lingkungan
        /// </summary>
        public string EnvironmentalImpact { get; set; }
        
        /// <summary>
        /// Action taken atau rencana cleanup
        /// </summary>
        public string ActionTaken { get; set; }
        
        // Relasi dengan user yang report
        public int UserID { get; set; }
        public User User { get; set; }
        
        // Optional: Relasi dengan kapal yang melakukan observasi
        public int? ShipID { get; set; }
        public Ship Ship { get; set; }
        
        // ===========================================
        // POLYMORPHISM: Method Overriding
        // ===========================================
        /// <summary>
        /// Override GenerateReport() untuk format khusus Pollution Report
        /// </summary>
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
        
        /// <summary>
        /// Override GetSummary() untuk summary singkat
        /// </summary>
        public override string GetSummary()
        {
            return base.GetSummary() + $" - {WasteType} ({Severity}) - {Status}";
        }
        
        /// <summary>
        /// Override Validate() dengan validasi khusus pollution report
        /// </summary>
        public override bool Validate()
        {
            return base.Validate() &&
                   !string.IsNullOrEmpty(WasteType) &&
                   !string.IsNullOrEmpty(Quantity) &&
                   !string.IsNullOrEmpty(Severity) &&
                   Latitude >= -90 && Latitude <= 90 &&
                   Longitude >= -180 && Longitude <= 180;
        }
        
        // ===========================================
        // Method khusus untuk Pollution Report
        // ===========================================
        
        /// <summary>
        /// Cek apakah polusi butuh immediate action
        /// </summary>
        public bool RequiresImmediateAction()
        {
            return Severity?.ToLower() == "critical" || 
                   Severity?.ToLower() == "high" ||
                   WasteType?.ToLower().Contains("oil") == true ||
                   WasteType?.ToLower().Contains("chemical") == true;
        }
        
        /// <summary>
        /// Cek apakah report sudah punya foto bukti
        /// </summary>
        public bool HasPhotos()
        {
            return !string.IsNullOrEmpty(PhotoPaths);
        }
        
        /// <summary>
        /// Get array of photo paths
        /// </summary>
        public string[] GetPhotoPaths()
        {
            if (string.IsNullOrEmpty(PhotoPaths))
                return new string[0];
            
            return PhotoPaths.Split(';', System.StringSplitOptions.RemoveEmptyEntries);
        }
        
        /// <summary>
        /// Get koordinat dalam format string
        /// </summary>
        public string GetCoordinatesString()
        {
            return $"{Latitude:F6}, {Longitude:F6}";
        }
        
        /// <summary>
        /// Calculate distance dari koordinat tertentu (dalam km)
        /// </summary>
        public double CalculateDistanceFrom(double lat, double lon)
        {
            // Haversine formula untuk hitung jarak
            const double R = 6371; // Radius bumi dalam km
            
            double dLat = ToRadians(lat - Latitude);
            double dLon = ToRadians(lon - Longitude);
            
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(Latitude)) * Math.Cos(ToRadians(lat)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            
            return R * c;
        }
        
        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}
