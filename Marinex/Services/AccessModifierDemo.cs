using System;
using System.Collections.Generic;
using Marinex.Models;

namespace Marinex.Services
{
    // ===========================================
    // ACCESS MODIFIER: INTERNAL - Class hanya bisa diakses dalam assembly yang sama
    // ===========================================
    // Class ini hanya bisa diakses dari class lain dalam assembly Marinex
    // Tidak bisa diakses dari assembly lain
    internal class AccessModifierDemo  // <-- ACCESS MODIFIER: internal
    {
        // ===========================================
        // ACCESS MODIFIER: PRIVATE - Hanya bisa diakses dalam class ini
        // ===========================================
        private string _demoMessage;  // <-- ACCESS MODIFIER: private
        
        // ===========================================
        // ACCESS MODIFIER: PROTECTED - Bisa diakses dari class ini dan derived classes
        // ===========================================
        protected string _protectedData;  // <-- ACCESS MODIFIER: protected
        
        // ===========================================
        // ACCESS MODIFIER: PUBLIC - Bisa diakses dari mana saja
        // ===========================================
        public string PublicData { get; set; }  // <-- ACCESS MODIFIER: public
        
        // ===========================================
        // ACCESS MODIFIER: INTERNAL - Bisa diakses dalam assembly yang sama
        // ===========================================
        internal string InternalData { get; set; }  // <-- ACCESS MODIFIER: internal
        
        public AccessModifierDemo()
        {
            _demoMessage = "Access Modifier Demonstration";
            _protectedData = "Protected Data";
            PublicData = "Public Data";
            InternalData = "Internal Data";
        }
        
        // ===========================================
        // ACCESS MODIFIER: PRIVATE - Method hanya bisa dipanggil dari dalam class ini
        // ===========================================
        private string GetPrivateMessage()  // <-- ACCESS MODIFIER: private
        {
            return _demoMessage;
        }
        
        // ===========================================
        // ACCESS MODIFIER: PROTECTED - Method bisa dipanggil dari child classes
        // ===========================================
        protected string GetProtectedData()  // <-- ACCESS MODIFIER: protected
        {
            return _protectedData;
        }
        
        // ===========================================
        // ACCESS MODIFIER: INTERNAL - Method bisa dipanggil dari class lain dalam assembly
        // ===========================================
        internal string GetInternalData()  // <-- ACCESS MODIFIER: internal
        {
            return InternalData;
        }
        
        // ===========================================
        // ACCESS MODIFIER: PUBLIC - Method bisa dipanggil dari mana saja
        // ===========================================
        public string DemonstrateAccessModifiers()  // <-- ACCESS MODIFIER: public
        {
            var result = new List<string>();
            result.Add("=== ACCESS MODIFIER DEMONSTRATION ===");
            result.Add("");
            result.Add($"1. PRIVATE: {GetPrivateMessage()}");  // <-- Memanggil private method (dalam class yang sama)
            result.Add($"2. PROTECTED: {GetProtectedData()}");  // <-- Memanggil protected method (dalam class yang sama)
            result.Add($"3. PUBLIC: {PublicData}");  // <-- Mengakses public property
            result.Add($"4. INTERNAL: {GetInternalData()}");  // <-- Memanggil internal method (dalam assembly yang sama)
            result.Add("");
            result.Add("=== PENJELASAN ACCESS MODIFIER ===");
            result.Add("1. PRIVATE: Hanya bisa diakses dalam class yang sama");
            result.Add("2. PROTECTED: Bisa diakses dari class ini dan derived classes");
            result.Add("3. PUBLIC: Bisa diakses dari mana saja");
            result.Add("4. INTERNAL: Bisa diakses dalam assembly yang sama");
            
            return string.Join("\n", result);
        }
    }
    
    // ===========================================
    // ACCESS MODIFIER: PUBLIC - Class bisa diakses dari assembly lain
    // ===========================================
    // Class ini bisa diakses dari mana saja, termasuk dari assembly lain
    public class AccessModifierDemoService  // <-- ACCESS MODIFIER: public
    {
        private AccessModifierDemo _demo;
        
        public AccessModifierDemoService()
        {
            // INTERNAL: Bisa mengakses internal class karena dalam assembly yang sama
            _demo = new AccessModifierDemo();  // <-- INTERNAL: Bisa mengakses internal class
        }
        
        // ===========================================
        // ACCESS MODIFIER: PUBLIC - Method bisa dipanggil dari mana saja
        // ===========================================
        public string DemonstrateAccessModifiers()  // <-- ACCESS MODIFIER: public
        {
            // Memanggil public method dari internal class
            return _demo.DemonstrateAccessModifiers();  // <-- PUBLIC: Memanggil public method
        }
        
        // ===========================================
        // ACCESS MODIFIER: PUBLIC - Method untuk menunjukkan protected access
        // ===========================================
        public string DemonstrateProtectedAccess(BaseReport report)  // <-- ACCESS MODIFIER: public
        {
            var result = new List<string>();
            result.Add("=== PROTECTED ACCESS MODIFIER DEMONSTRATION ===");
            result.Add("");
            
            // PROTECTED: Mengakses protected field melalui public property
            report.ReportStatus = "Active";  // <-- PUBLIC property mengakses protected field
            report.Version = 1;  // <-- PUBLIC property mengakses protected field
            
            result.Add($"Report Status (via protected): {report.ReportStatus}");  // <-- Mengakses protected melalui public property
            result.Add($"Report Version (via protected): {report.Version}");  // <-- Mengakses protected melalui public property
            result.Add("");
            result.Add("Penjelasan:");
            result.Add("- _reportStatus dan _version adalah PROTECTED fields di BaseReport");
            result.Add("- Child classes (SafetyReport, MaintenanceReport, WeatherReport) bisa mengakses protected fields");
            result.Add("- Class lain bisa mengakses protected fields melalui PUBLIC properties");
            
            return string.Join("\n", result);
        }
        
        // ===========================================
        // ACCESS MODIFIER: PUBLIC - Method untuk menunjukkan internal access
        // ===========================================
        public string DemonstrateInternalAccess()  // <-- ACCESS MODIFIER: public
        {
            var result = new List<string>();
            result.Add("=== INTERNAL ACCESS MODIFIER DEMONSTRATION ===");
            result.Add("");
            
            // INTERNAL: Bisa mengakses internal class dan internal methods
            var reportService = new ReportService();  // <-- PUBLIC class bisa diakses
            reportService.IncrementProcessedReports();  // <-- INTERNAL method bisa diakses karena dalam assembly yang sama
            var count = reportService.GetTotalProcessedReports();  // <-- INTERNAL method bisa diakses karena dalam assembly yang sama
            var instanceCount = ReportService.GetInstanceCount();  // <-- INTERNAL static method bisa diakses
            
            result.Add($"Total Processed Reports (internal method): {count}");
            result.Add($"Instance Count (internal static method): {instanceCount}");
            result.Add("");
            result.Add("Penjelasan:");
            result.Add("- GetTotalProcessedReports() dan IncrementProcessedReports() adalah INTERNAL methods");
            result.Add("- INTERNAL methods bisa diakses dari class lain dalam assembly yang sama");
            result.Add("- INTERNAL methods TIDAK bisa diakses dari assembly lain");
            
            return string.Join("\n", result);
        }
    }
}

