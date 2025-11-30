using System;

namespace Marinex.Models
{
 
    public abstract class BaseReport
    {
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
        
        public abstract string GenerateReport();
        
        public virtual string GetSummary()
        {
            return $"Report at {Location} created on {CreatedAt:yyyy-MM-dd}";
        }
        
        public virtual bool Validate()
        {
            return !string.IsNullOrEmpty(Location);
        }
    }
}

