namespace Marinex.Models
{
    /// <summary>
    /// Child class yang inherit dari BaseReport - Demonstrasi Inheritance
    /// Implementasi khusus untuk Maintenance Report (berbeda dengan SafetyReport)
    /// </summary>
    public class MaintenanceReport : BaseReport
    {
        // Encapsulation: Properties khusus untuk Maintenance Report
        public string EquipmentName { get; set; }
        public string IssueDescription { get; set; }
        public string Priority { get; set; }
        public int ShipID { get; set; }
        public int UserID { get; set; }
        
        // Polymorphism: Override abstract method dengan implementasi berbeda dari SafetyReport
        public override string GenerateReport()
        {
            return $"MAINTENANCE REPORT\n" +
                   $"Location: {Location}\n" +
                   $"Equipment: {EquipmentName}\n" +
                   $"Issue: {IssueDescription}\n" +
                   $"Priority: {Priority}\n" +
                   $"Reported on: {CreatedAt:yyyy-MM-dd HH:mm:ss}";
        }
        
        // Polymorphism: Override virtual method dengan behavior berbeda
        public override string GetSummary()
        {
            return base.GetSummary() + $" - Equipment: {EquipmentName} (Priority: {Priority})";
        }
        
        // Method khusus untuk Maintenance Report
        public bool IsUrgent()
        {
            return Priority?.ToLower() == "urgent" || Priority?.ToLower() == "high";
        }
    }
}

