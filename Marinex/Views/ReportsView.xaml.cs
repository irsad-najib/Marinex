using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Marinex.Models;
using Marinex.Services;
using Npgsql;

namespace Marinex.Views
{
    public partial class ReportsView : Window
    {
        private readonly ReportService _reportService;
        private readonly MaintenanceAIService _aiService;
        private readonly int _userId;
        private List<ReportListItem> _allReports;
        
        // Connection string similar to other services
        private string GetConnectionString()
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = "aws-1-ap-south-1.pooler.supabase.com",
                Port = 5432,
                Database = "postgres",
                Username = "postgres.ihgwotixfliblkyoqujl",
                Password = "Marinex123.",
                SslMode = SslMode.Require,
                TrustServerCertificate = true,
                Timeout = 30,
                CommandTimeout = 30
            };
            return builder.ConnectionString;
        }

        public ReportsView(int userId)
        {
            InitializeComponent();
            _reportService = new ReportService();
            _aiService = new MaintenanceAIService();
            _userId = userId;
            // Use async load
            LoadReportsAsync();
        }

        private async void LoadReportsAsync()
        {
            try 
            {
                _allReports = new List<ReportListItem>();
                
                // Load database data (All Types)
                await LoadDatabaseReports();
                
                // Add Sample Data only if DB is completely empty
                if (_allReports.Count == 0) 
                {
                    AddSampleData();
                }

                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading reports: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Fallback to sample data
                AddSampleData();
                ApplyFilter();
            }
        }

        private async Task LoadDatabaseReports()
        {
            using var conn = new NpgsqlConnection(GetConnectionString());
            try
            {
                await conn.OpenAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB Connection Error: {ex.Message}");
                return;
            }

            // 1. Load Pollution Reports
            try
            {
                string pollutionQuery = @"
                    SELECT location, category, severity, description, userid 
                    FROM wastereport 
                    ORDER BY reportid DESC 
                    LIMIT 50";

                using (var cmd = new NpgsqlCommand(pollutionQuery, conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string location = reader.IsDBNull(0) ? "Unknown" : reader.GetString(0);
                        string category = reader.IsDBNull(1) ? "Unknown" : reader.GetString(1);
                        string severity = reader.IsDBNull(2) ? "Medium" : reader.GetString(2);
                        string description = reader.IsDBNull(3) ? "" : reader.GetString(3);
                        
                        double lat = 0, lon = 0;
                        ParseCoordinates(location, out lat, out lon);

                        var report = new PollutionReport
                        {
                            Location = location,
                            WasteType = category,
                            Severity = severity,
                            Description = description,
                            Latitude = lat,
                            Longitude = lon,
                            CreatedAt = DateTime.Now,
                            Status = "Reported"
                        };

                        _allReports.Add(new ReportListItem
                        {
                            Title = $"{category} at {location}",
                            Summary = description.Length > 50 ? description.Substring(0, 50) + "..." : description,
                            Type = "Pollution",
                            DateString = "Recently",
                            ReportType = "Pollution",
                            Report = report
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pollution Load Error: {ex.Message}");
            }

            // 2. Load Safety Reports
            try
            {
                string safetyQuery = @"
                    SELECT incident_type, location, severity, description, user_id
                    FROM safetyreport 
                    ORDER BY report_id DESC 
                    LIMIT 50";

                using (var cmd = new NpgsqlCommand(safetyQuery, conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string incidentType = reader.IsDBNull(0) ? "Unknown" : reader.GetString(0);
                        string location = reader.IsDBNull(1) ? "Unknown" : reader.GetString(1);
                        string severity = reader.IsDBNull(2) ? "Medium" : reader.GetString(2);
                        string description = reader.IsDBNull(3) ? "" : reader.GetString(3);

                        var report = new SafetyReport
                        {
                            IncidentType = incidentType,
                            Location = location,
                            Severity = severity,
                            Description = description,
                            CreatedAt = DateTime.Now
                        };

                        _allReports.Add(new ReportListItem
                        {
                            Title = $"{incidentType} - {location}",
                            Summary = description.Length > 50 ? description.Substring(0, 50) + "..." : description,
                            Type = "Safety",
                            DateString = "Recently",
                            ReportType = "Safety",
                            Report = report
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Safety Load Error: {ex.Message}");
            }

            // 3. Load Maintenance Reports
            try
            {
                // Note: using ship_id as per latest migration
                string maintenanceQuery = @"
                    SELECT task_name, status, due_date, description, ship_id
                    FROM maintenance 
                    ORDER BY maintenanceid DESC 
                    LIMIT 50";

                using (var cmd = new NpgsqlCommand(maintenanceQuery, conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string taskName = reader.IsDBNull(0) ? "Unknown Task" : reader.GetString(0);
                        string status = reader.IsDBNull(1) ? "Pending" : reader.GetString(1);
                        // due_date index 2 skipped
                        string description = reader.IsDBNull(3) ? "" : reader.GetString(3);
                        int shipId = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);

                        var report = new MaintenanceReport
                        {
                            EquipmentName = taskName,
                            Location = $"Ship ID: {shipId}",
                            IssueDescription = $"{description}\nStatus: {status}",
                            ShipID = shipId,
                            CreatedAt = DateTime.Now,
                            Priority = "Medium" // Default
                        };

                        _allReports.Add(new ReportListItem
                        {
                            Title = $"{taskName} (Ship #{shipId})",
                            Summary = description.Length > 100 ? description.Substring(0, 100) + "..." : description,
                            Type = "Maintenance",
                            DateString = "Recently",
                            ReportType = "Maintenance",
                            Report = report
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Maintenance Load Error: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ParseCoordinates(string locationStr, out double lat, out double lon)
        {
            lat = 0; lon = 0;
            try 
            {
                if (locationStr.Contains("(") && locationStr.Contains(")"))
                {
                    int start = locationStr.LastIndexOf("(") + 1;
                    int end = locationStr.LastIndexOf(")");
                    string coordPart = locationStr.Substring(start, end - start);
                    string[] parts = coordPart.Split(',');
                    if (parts.Length == 2)
                    {
                        double.TryParse(parts[0].Trim(), out lat);
                        double.TryParse(parts[1].Trim(), out lon);
                    }
                }
            }
            catch { /* Ignore parsing errors */ }
        }

        private void AddSampleData()
        {
            _allReports.Add(new ReportListItem
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
                    Description = "Main engine suddenly stopped. Crew investigating the cause.\nAction Taken: Switched to auxiliary engine.",
                    UserID = 1,
                    CreatedAt = DateTime.Now.AddDays(-1)
                }
            });

            _allReports.Add(new ReportListItem
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
                    IssueDescription = "Task Type: Routine Inspection\nStatus: Completed\nFindings: Minor corrosion detected.",
                    ShipID = 1,
                    UserID = 1,
                    CreatedAt = DateTime.Now.AddDays(-3)
                }
            });
        }

        private void ApplyFilter()
        {
            if (_allReports == null) return;

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
                
                string reportContent = item.Report.GenerateReport();

                // AI Integration for Maintenance Reports
                if (item.Report is MaintenanceReport maintenanceReport)
                {
                    // Check if AI analysis is already present in the description (from Create Report)
                    if (!reportContent.Contains("[AI ANALYSIS]"))
                    {
                        reportContent += "\n\n" + GenerateAIAnalysis(maintenanceReport);
                    }
                }

                txtReportDetail.Text = reportContent;
            }
            else
            {
                pnlEmptyState.Visibility = Visibility.Visible;
                txtReportDetail.Visibility = Visibility.Collapsed;
            }
        }

        private string GenerateAIAnalysis(MaintenanceReport report)
        {
            try 
            {
                // Simulate ship data based on report details for AI demonstration
                // In a real app, we would fetch the Ship and its History from DB
                var ship = new Ship 
                { 
                    ShipID = report.ShipID, 
                    ShipName = $"Ship #{report.ShipID}",
                    StartVoyage = DateTime.Now.AddYears(-5) // Assume 5 year old ship default
                };

                var history = new List<Maintenance>();
                double engineHours = 2000;
                double fuelConsumption = 100; // Baseline

                // Adjust simulation based on report priority/severity to show AI capabilities
                if (report.IsUrgent())
                {
                    // Simulate critical conditions
                    engineHours = 6000; // Critical > 5000
                    fuelConsumption = 130; // +30% deviation
                    history.Add(new Maintenance { Date = DateTime.Now.AddDays(-200) }); // > 180 days ago
                }
                else if (report.Priority == "Medium")
                {
                    // Simulate moderate conditions
                    engineHours = 3000;
                    fuelConsumption = 115; // +15%
                    history.Add(new Maintenance { Date = DateTime.Now.AddDays(-90) });
                }
                else
                {
                    // Good conditions
                    engineHours = 1000;
                    fuelConsumption = 102;
                    history.Add(new Maintenance { Date = DateTime.Now.AddDays(-30) });
                }

                var prediction = _aiService.PredictMaintenance(ship, history, engineHours, fuelConsumption);

                return "🤖 AI MAINTENANCE ANALYSIS\n" + 
                       "========================\n" +
                       prediction.ToString();
            }
            catch (Exception ex)
            {
                return $"AI Analysis Unavailable: {ex.Message}";
            }
        }

        private void btnNewReport_Click(object sender, RoutedEventArgs e)
        {
            var createReportWindow = new CreateReportView(_userId);
            createReportWindow.ShowDialog();
            LoadReportsAsync(); // Refresh list after creating report
        }
    }

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
// Force Update 1
