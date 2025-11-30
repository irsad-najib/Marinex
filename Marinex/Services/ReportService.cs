using System;
using System.Collections.Generic;
using Marinex.Models;

namespace Marinex.Services
{
    public class ReportService
    {
        public string ProcessReport(BaseReport report)
        {
            if (!report.Validate())
            {
                return "Report validation failed";
            }

            return report.GenerateReport();
        }

        public string GetReportSummary(BaseReport report)
        {
            return report.GetSummary();
        }

        public bool CheckEmergencyResponse(SafetyReport safetyReport)
        {
            return safetyReport.RequiresEmergencyResponse();
        }

        public bool CheckUrgency(MaintenanceReport maintenanceReport)
        {
            return maintenanceReport.IsUrgent();
        }

        public List<string> ProcessMultipleReports(List<BaseReport> reports)
        {
            var results = new List<string>();
            
            foreach (var report in reports)
            {
                results.Add(report.GenerateReport());
            }
            
            return results;
        }
    }
}

