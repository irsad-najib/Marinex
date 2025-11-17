using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Marinex.Models;
using Marinex.Services;

namespace Marinex.Views
{
    public partial class ReportsView : Window
    {
        private readonly ReportService _reportService;
        private readonly int _userId;
        private List<ReportListItem> _allReports;

        public ReportsView(int userId)
        {
            InitializeComponent();
            _reportService = new ReportService();
            _userId = userId;
            LoadReports();
        }

        private void LoadReports()
        {
            // TODO: Load from database
            // For now, create sample data
            _allReports = new List<ReportListItem>
            {
                new ReportListItem
                {
                    Title = "Plastic Waste in Pacific Ocean",
                    Summary = "Large amount of plastic debris detected at coordinates 35.6892°N, 139.6917°E",
                    Type = "Pollution",
                    DateString = "2 hours ago",
                    ReportType = "Pollution",
                    Report = new PollutionReport
                    {
                        Location = "Pacific Ocean - Tokyo Bay",
                        WasteType = "Plastic Waste",
                        Quantity = "Large",
                        Severity = "High",
                        Latitude = 35.6892,
                        Longitude = 139.6917,
                        Description = "Large accumulation of plastic bottles and bags floating in the water.",
                        CreatedAt = DateTime.Now.AddHours(-2)
                    }
                },
                new ReportListItem
                {
                    Title = "Engine Failure - MV Sea Explorer",
                    Summary = "Main engine stopped unexpectedly during voyage",
                    Type = "Safety",
                    DateString = "Yesterday",
                    ReportType = "Safety",
                    Report = new SafetyReport
                    {
                        Location = "Indian Ocean",
                        IncidentType = "Equipment Failure",
                        Severity = "Major",
                        Description = "Main engine suddenly stopped. Crew investigating the cause.\nAction Taken: Switched to auxiliary engine. Called for technical support.",
                        UserID = 1,
                        CreatedAt = DateTime.Now.AddDays(-1)
                    }
                },
                new ReportListItem
                {
                    Title = "Routine Hull Inspection",
                    Summary = "Scheduled hull inspection completed",
                    Type = "Maintenance",
                    DateString = "3 days ago",
                    ReportType = "Maintenance",
                    Report = new MaintenanceReport
                    {
                        Location = "Singapore Port",
                        EquipmentName = "Ship Hull",
                        Priority = "Medium",
                        IssueDescription = "Task Type: Routine Inspection\nStatus: Completed\nAnnual hull inspection and cleaning\nFindings: Minor corrosion detected on starboard side. Scheduled for repair next week.",
                        ShipID = 1,
                        UserID = 1,
                        CreatedAt = DateTime.Now.AddDays(-3)
                    }
                }
            };

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var filtered = _allReports.AsEnumerable();

            // Apply type filter
            if (rbPollution.IsChecked == true)
                filtered = filtered.Where(r => r.ReportType == "Pollution");
            else if (rbSafety.IsChecked == true)
                filtered = filtered.Where(r => r.ReportType == "Safety");
            else if (rbMaintenance.IsChecked == true)
                filtered = filtered.Where(r => r.ReportType == "Maintenance");

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                var searchTerm = txtSearch.Text.ToLower();
                filtered = filtered.Where(r => 
                    r.Title.ToLower().Contains(searchTerm) || 
                    r.Summary.ToLower().Contains(searchTerm));
            }

            lstReports.ItemsSource = filtered.ToList();
        }

        private void FilterChanged(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void lstReports_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstReports.SelectedItem is ReportListItem item)
            {
                pnlEmptyState.Visibility = Visibility.Collapsed;
                txtReportDetail.Visibility = Visibility.Visible;
                txtReportDetail.Text = item.Report.GenerateReport();
            }
            else
            {
                pnlEmptyState.Visibility = Visibility.Visible;
                txtReportDetail.Visibility = Visibility.Collapsed;
            }
        }

        private void btnNewReport_Click(object sender, RoutedEventArgs e)
        {
            var createReportWindow = new CreateReportView(_userId);
            createReportWindow.ShowDialog();
            LoadReports(); // Refresh list after creating report
        }
    }

    // Helper class for ListView binding
    public class ReportListItem
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Type { get; set; }
        public string DateString { get; set; }
        public string ReportType { get; set; }
        public BaseReport Report { get; set; }
    }
}
