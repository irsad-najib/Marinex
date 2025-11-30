using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Marinex.Models;
using Npgsql;

namespace Marinex.Services
{
    public class PollutionDataService
    {
        // Singleton pattern biar data bisa diakses global & update realtime
        private static PollutionDataService _instance;
        public static PollutionDataService Instance => _instance ??= new PollutionDataService();

        private const string DATA_FILE_PATH = "Assets/marine_debris_data.json";
        private List<PollutionReport> _cachedReports;
        
        // Use SupabaseService connection string strategy
        private string _connectionString;

        // Event buat notifikasi kalau ada report baru masuk
        public event EventHandler<PollutionReport> OnReportAdded;

        public PollutionDataService()
        {
            _cachedReports = new List<PollutionReport>();
            // Re-use the connection string logic from SupabaseService (or inject it properly)
            _connectionString = GetConnectionString();
        }
        
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

        public async Task<List<PollutionReport>> LoadPollutionDataAsync()
        {
            // Kalau sudah ada cache (termasuk data baru yang diinput user), return cache
            if (_cachedReports != null && _cachedReports.Count > 0)
            {
                return _cachedReports;
            }

            try
            {
                // 1. Load initial dummy data from JSON
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string fullPath = Path.Combine(baseDir, DATA_FILE_PATH);
                
                // Fallback logic
                if (!File.Exists(fullPath))
                {
                    string projectRoot = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\"));
                    fullPath = Path.Combine(projectRoot, "Marinex", DATA_FILE_PATH);
                }

                if (File.Exists(fullPath))
                {
                    string jsonContent = await File.ReadAllTextAsync(fullPath);
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var rawData = JsonSerializer.Deserialize<List<PollutionDataRaw>>(jsonContent, options);
                    
                    var initialReports = ConvertToReports(rawData);
                    _cachedReports.AddRange(initialReports);
                }
                
                // 2. Load real data from Supabase/PostgreSQL and merge
                await LoadFromDatabaseAsync();

                return _cachedReports;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading pollution data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<PollutionReport>();
            }
        }

        private async Task LoadFromDatabaseAsync()
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                // Ensure tables exist (Auto-Migration for MVP)
                await InitializeTablesAsync(conn);

                // Query wastereport table
                string query = @"
                    SELECT location, category, severity, description, userid 
                    FROM wastereport 
                    ORDER BY reportid DESC 
                    LIMIT 100";

                using var cmd = new NpgsqlCommand(query, conn);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    // ... existing read logic ...
                    string location = reader.IsDBNull(0) ? "Unknown" : reader.GetString(0);
                    string category = reader.IsDBNull(1) ? "Unknown" : reader.GetString(1);
                    string severity = reader.IsDBNull(2) ? "Medium" : reader.GetString(2);
                    string description = reader.IsDBNull(3) ? "" : reader.GetString(3);
                    
                    double lat = 0, lon = 0;
                    ParseCoordinates(location, out lat, out lon);

                    if (lat != 0 && lon != 0)
                    {
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
                        
                        if (!_cachedReports.Exists(r => r.Latitude == lat && r.Longitude == lon))
                        {
                            _cachedReports.Add(report);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB Load Error in PollutionDataService: {ex.Message}");
            }
        }

        private async Task InitializeTablesAsync(NpgsqlConnection conn)
        {
            // Create tables if they don't exist
            string createWasteTable = @"
                CREATE TABLE IF NOT EXISTS wastereport (
                    reportid SERIAL PRIMARY KEY,
                    reporter TEXT,
                    location TEXT,
                    category TEXT,
                    severity TEXT,
                    description TEXT,
                    userid INTEGER,
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                );";

            string createSafetyTable = @"
                CREATE TABLE IF NOT EXISTS safetyreport (
                    report_id SERIAL PRIMARY KEY,
                    incident_type TEXT,
                    location TEXT,
                    severity TEXT,
                    description TEXT,
                    user_id INTEGER,
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                );";

            string createMaintenanceTable = @"
                CREATE TABLE IF NOT EXISTS maintenance (
                    maintenance_id SERIAL PRIMARY KEY,
                    ship_id INTEGER,
                    task_name TEXT,
                    status TEXT,
                    due_date TIMESTAMP,
                    description TEXT,
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                );";

            using (var cmd = new NpgsqlCommand(createWasteTable, conn)) await cmd.ExecuteNonQueryAsync();
            using (var cmd = new NpgsqlCommand(createSafetyTable, conn)) await cmd.ExecuteNonQueryAsync();
            using (var cmd = new NpgsqlCommand(createMaintenanceTable, conn)) await cmd.ExecuteNonQueryAsync();

            // Migration: Ensure ship_id exists in maintenance table (for existing tables)
            try 
            {
                string alterTableQuery = @"
                    ALTER TABLE maintenance ADD COLUMN IF NOT EXISTS ship_id INTEGER DEFAULT 0;
                    ALTER TABLE maintenance ADD COLUMN IF NOT EXISTS task_name TEXT;
                    ALTER TABLE maintenance ADD COLUMN IF NOT EXISTS status TEXT DEFAULT 'Pending';
                    ALTER TABLE maintenance ADD COLUMN IF NOT EXISTS due_date TIMESTAMP;
                    ALTER TABLE maintenance ADD COLUMN IF NOT EXISTS description TEXT;
                ";
                using (var cmd = new NpgsqlCommand(alterTableQuery, conn)) await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Migration Warning: {ex.Message}");
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

        // Method buat nambah report baru dari User (Crowdsourcing)
        // Updated: Saves to Database NOW!
        public async Task<bool> AddReportAsync(PollutionReport report)
        {
            try 
            {
                if (_cachedReports == null)
                {
                    _cachedReports = new List<PollutionReport>();
                }

                // 1. Update Local Cache & UI immediately (Optimistic UI)
                _cachedReports.Add(report);
                OnReportAdded?.Invoke(this, report);

                // 2. Save to PostgreSQL / Supabase
                await SaveReportToDatabaseAsync(report);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save report to database: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                // Still returns true because UI updated successfully (offline mode behavior)
                return false;
            }
        }
        
        private async Task SaveReportToDatabaseAsync(PollutionReport report)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            // Auto-migrate tables before saving
            await InitializeTablesAsync(conn);

            // Mapping PollutionReport to wastereport table structure
            string query = @"
                INSERT INTO wastereport (reporter, location, category, severity, description, userid)
                VALUES (@reporter, @location, @category, @severity, @description, @userid)
            ";

            using var cmd = new NpgsqlCommand(query, conn);
            
            // Use User object name if available, else default
            string reporterName = report.User?.UserName ?? "Anonymous"; 
            
            // Combine Lat/Lon into location string if exact columns missing, or use the Location name
            string locationStr = $"{report.Location} ({report.Latitude:F5}, {report.Longitude:F5})";

            cmd.Parameters.AddWithValue("reporter", reporterName);
            cmd.Parameters.AddWithValue("location", locationStr);
            cmd.Parameters.AddWithValue("category", report.WasteType ?? "Unknown");
            cmd.Parameters.AddWithValue("severity", report.Severity ?? "Medium");
            cmd.Parameters.AddWithValue("description", report.Description ?? "");
            cmd.Parameters.AddWithValue("userid", report.UserID);

            await cmd.ExecuteNonQueryAsync();
        }

        // NEW: Helper to save Safety Report to DB
        public async Task SaveSafetyReportAsync(SafetyReport report)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            // Auto-migrate tables before saving
            await InitializeTablesAsync(conn);

            string query = @"
                INSERT INTO safetyreport (incident_type, location, severity, description, user_id)
                VALUES (@incident_type, @location, @severity, @description, @user_id)
            ";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("incident_type", report.IncidentType);
            cmd.Parameters.AddWithValue("location", report.Location);
            cmd.Parameters.AddWithValue("severity", report.Severity);
            cmd.Parameters.AddWithValue("description", report.Description);
            cmd.Parameters.AddWithValue("user_id", report.UserID);

            await cmd.ExecuteNonQueryAsync();
        }

        // NEW: Helper to save Maintenance Report to DB
        public async Task SaveMaintenanceReportAsync(MaintenanceReport report)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            // Auto-migrate tables before saving
            await InitializeTablesAsync(conn);

            string query = @"
                INSERT INTO maintenance (ship_id, task_name, status, due_date, description)
                VALUES (@ship_id, @task_name, @status, @due_date, @description)
            ";

            using var cmd = new NpgsqlCommand(query, conn);
            // Assuming default ship_id if not selected (e.g. 1) or 0
            // Also assuming 'status' comes from IssueDescription parsing or default 'Pending'
            // We'll extract task name from EquipmentName for now
            
            int shipId = report.ShipID != 0 ? report.ShipID : 1; // Default/Fallback ship ID
            string status = "Pending"; // Default status

            cmd.Parameters.AddWithValue("ship_id", shipId);
            cmd.Parameters.AddWithValue("task_name", report.EquipmentName);
            cmd.Parameters.AddWithValue("status", status);
            cmd.Parameters.AddWithValue("due_date", DateTime.Now.AddDays(7)); // Default due date
            cmd.Parameters.AddWithValue("description", report.IssueDescription);

            await cmd.ExecuteNonQueryAsync();
        }

        private List<PollutionReport> ConvertToReports(List<PollutionDataRaw> rawData)
        {
            var reports = new List<PollutionReport>();
            if (rawData == null) return reports;

            foreach (var item in rawData)
            {
                reports.Add(new PollutionReport
                {
                    Location = $"Lat: {item.Latitude}, Lon: {item.Longitude}",
                    Latitude = item.Latitude,
                    Longitude = item.Longitude,
                    WasteType = item.Type,
                    Severity = item.Severity,
                    Description = item.Description,
                    CreatedAt = item.Date,
                    Status = "Reported",
                    Quantity = "N/A", 
                    UserID = 0 
                });
            }
            return reports;
        }

        private class PollutionDataRaw
        {
            public string Id { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string Type { get; set; }
            public string Severity { get; set; }
            public string Description { get; set; }
            public string ReportedBy { get; set; }
            public DateTime Date { get; set; }
        }
    }
}
