using System;

namespace Marinex.Models
{
    /// <summary>
    /// ===========================================
    /// INHERITANCE: Base class untuk semua Report
    /// ===========================================
    /// Base class abstract yang menjadi parent class untuk semua jenis report.
    /// Child classes: SafetyReport, MaintenanceReport, WeatherReport
    /// </summary>
    public abstract class BaseReport
    {
        // ===========================================
        // ACCESS MODIFIER: PRIVATE - Hanya bisa diakses dalam class ini
        // ===========================================
        // ENCAPSULATION: Private fields dengan public properties
        // Data dienkapsulasi dalam private fields dan diakses melalui public properties
        private int _reportID;  // <-- ACCESS MODIFIER: private
        private string _location;  // <-- ACCESS MODIFIER: private
        private DateTime _createdAt;  // <-- ACCESS MODIFIER: private
        

        protected string _reportStatus;  // <-- ACCESS MODIFIER: protected - bisa diakses dari child classes
        protected int _version;  // <-- ACCESS MODIFIER: protected - bisa diakses dari child classes
        
        // ===========================================
        // ACCESS MODIFIER: PROTECTED - Protected method
        // ===========================================
        protected virtual string GetReportStatus()  // <-- ACCESS MODIFIER: protected
        {
            return _reportStatus ?? "Draft";
        }
        
        protected void SetReportStatus(string status)  // <-- ACCESS MODIFIER: protected
        {
            _reportStatus = status;
        }
        
        // ENCAPSULATION: Public property untuk mengakses private field _reportID
        public int ReportID 
        { 
            get => _reportID; 
            set => _reportID = value; 
        }
        
        // ENCAPSULATION: Public property dengan validasi untuk mengakses private field _location
        public string Location 
        { 
            get => _location; 
            set => _location = value ?? throw new ArgumentNullException(nameof(Location)); 
        }
        
        // ENCAPSULATION: Public property untuk mengakses private field _createdAt
        public DateTime CreatedAt 
        { 
            get => _createdAt; 
            set => _createdAt = value; 
        }
        
        // ===========================================
        // POLYMORPHISM: Abstract method untuk method overriding
        // ===========================================
        // Method ini HARUS di-override oleh child class (SafetyReport, MaintenanceReport, WeatherReport)
        // Setiap child class akan memiliki implementasi yang berbeda (POLYMORPHISM)
        public abstract string GenerateReport();
        
        // ===========================================
        // POLYMORPHISM: Virtual method yang bisa di-override
        // ===========================================
        // Method ini BISA di-override oleh child class dengan implementasi yang berbeda
        public virtual string GetSummary()
        {
            return $"Report at {Location} created on {CreatedAt:yyyy-MM-dd}";
        }
        
        // ===========================================
        // POLYMORPHISM: Virtual method untuk validasi
        // ===========================================
        // Method ini BISA di-override oleh child class dengan validasi yang lebih spesifik
        public virtual bool Validate()
        {
            return !string.IsNullOrEmpty(Location);
        }
        
        // ===========================================
        // ACCESS MODIFIER: PUBLIC - Bisa diakses dari mana saja
        // ===========================================
        // Public method untuk mengakses protected field melalui method
        public string ReportStatus 
        { 
            get => GetReportStatus();  // <-- Mengakses protected method
            set => SetReportStatus(value);  // <-- Mengakses protected method
        }
        
        public int Version 
        { 
            get => _version;  // <-- Mengakses protected field
            set => _version = value;  // <-- Mengakses protected field
        }
    }
}

