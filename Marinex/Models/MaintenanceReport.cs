namespace Marinex.Models
{

    public class MaintenanceReport : BaseReport  
    {
        
        public string EquipmentName { get; set; }
        public string IssueDescription { get; set; }
        public string Priority { get; set; }
        public int ShipID { get; set; }
        public int UserID { get; set; }   

        public override string GenerateReport()  
        {
            return $"MAINTENANCE REPORT\n" +
                   $"Location: {Location}\n" +  
                   $"Equipment: {EquipmentName}\n" +
                   $"Issue: {IssueDescription}\n" +
                   $"Priority: {Priority}\n" +
                   $"Reported on: {CreatedAt:yyyy-MM-dd HH:mm:ss}";  }
        
        public override string GetSummary()  
        {
            return base.GetSummary() + $" - Equipment: {EquipmentName} (Priority: {Priority})";  
        }
        
        public bool IsUrgent()
        {
            return Priority?.ToLower() == "urgent" || Priority?.ToLower() == "high";
        }
    }
}

