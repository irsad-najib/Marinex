using System;

namespace Marinex.Models
{
    /// <summary>
    /// Base class untuk Inheritance demonstration
    /// Menggunakan konsep Inheritance untuk semua jenis Report
    /// </summary>
    public abstract class BaseReport
    {
        // Encapsulation: private fields dengan public properties
        private int _reportID;
        private string _location;
        private DateTime _createdAt;
        
        public int ReportID 
        { 
            get => _reportID; 
            set => _reportID = value; 
        }
        
        public string Location 
        { 
            get => _location; 
            set => _location = value ?? throw new ArgumentNullException(nameof(Location)); 
        }
        
        public DateTime CreatedAt 
        { 
            get => _createdAt; 
            set => _createdAt = value; 
        }
        
        // Abstract method yang harus diimplementasikan oleh child class (Polymorphism)
        public abstract string GenerateReport();
        
        // Virtual method yang bisa di-override oleh child class
        public virtual string GetSummary()
        {
            return $"Report at {Location} created on {CreatedAt:yyyy-MM-dd}";
        }
        
        // Method umum untuk validasi
        public virtual bool Validate()
        {
            return !string.IsNullOrEmpty(Location);
        }
    }
}

