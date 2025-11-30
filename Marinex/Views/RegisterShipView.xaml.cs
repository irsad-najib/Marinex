using System;
using System.Windows;
using Marinex.Models;
using Marinex.Services;
using Npgsql;

namespace Marinex.Views
{
    public partial class RegisterShipView : Window
    {
        private readonly int _userId;

        public RegisterShipView(int userId = 0)
        {
            InitializeComponent();
            _userId = userId;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            // 1. Validation
            if (string.IsNullOrWhiteSpace(txtShipName.Text))
            {
                MessageBox.Show("Ship Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMMSI.Text) || txtMMSI.Text.Length != 9 || !long.TryParse(txtMMSI.Text, out _))
            {
                MessageBox.Show("Please enter a valid 9-digit MMSI number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtCapacity.Text, out int capacity))
            {
                capacity = 0; // Default or error
            }

            // 2. Create Ship Object
            var ship = new Ship
            {
                ShipName = txtShipName.Text,
                ShipType = cmbShipType.Text,
                Owner = txtOwner.Text,
                MMSI = txtMMSI.Text,
                Capacity = capacity,
                Status = cmbStatus.Text,
                AISEnabled = chkAISEnabled.IsChecked ?? false,
                StartVoyage = cmbStatus.Text == "Sailing" ? DateTime.Now : (DateTime?)null
            };

            try
            {
                // 3. Save to Database
                await SaveShipToDatabase(ship);

                MessageBox.Show($"Ship '{ship.ShipName}' registered successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error registering ship: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async System.Threading.Tasks.Task SaveShipToDatabase(Ship ship)
        {
            // Use existing connection string logic (ideally centralized)
            string connectionString = GetConnectionString();

            using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();

            // Auto-migration: Ensure table exists with correct schema
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS ships (
                    shipid SERIAL PRIMARY KEY,
                    shipname TEXT NOT NULL,
                    shiptype TEXT,
                    owner TEXT,
                    capacity INTEGER,
                    status TEXT,
                    mmsi TEXT,
                    ais_enabled BOOLEAN DEFAULT TRUE,
                    startvoyage TIMESTAMP,
                    endvoyage TIMESTAMP,
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                );";

            using (var cmdTable = new NpgsqlCommand(createTableQuery, conn))
            {
                await cmdTable.ExecuteNonQueryAsync();
            }
            
            // Check if 'ais_enabled' column exists (migration for existing tables)
            try 
            {
                string alterQuery = "ALTER TABLE ships ADD COLUMN IF NOT EXISTS ais_enabled BOOLEAN DEFAULT TRUE; " +
                                    "ALTER TABLE ships ADD COLUMN IF NOT EXISTS mmsi TEXT;";
                using (var cmdAlter = new NpgsqlCommand(alterQuery, conn))
                {
                    await cmdAlter.ExecuteNonQueryAsync();
                }
            }
            catch { /* Ignore if columns exist */ }


            // Insert Query
            string insertQuery = @"
                INSERT INTO ships (shipname, shiptype, owner, capacity, status, mmsi, ais_enabled, startvoyage)
                VALUES (@shipname, @shiptype, @owner, @capacity, @status, @mmsi, @ais_enabled, @startvoyage)";

            using var cmd = new NpgsqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("shipname", ship.ShipName);
            cmd.Parameters.AddWithValue("shiptype", ship.ShipType ?? "");
            cmd.Parameters.AddWithValue("owner", ship.Owner ?? "");
            cmd.Parameters.AddWithValue("capacity", ship.Capacity);
            cmd.Parameters.AddWithValue("status", ship.Status ?? "Docked");
            cmd.Parameters.AddWithValue("mmsi", ship.MMSI ?? "");
            cmd.Parameters.AddWithValue("ais_enabled", ship.AISEnabled);
            
            if (ship.StartVoyage.HasValue)
                cmd.Parameters.AddWithValue("startvoyage", ship.StartVoyage.Value);
            else
                cmd.Parameters.AddWithValue("startvoyage", DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
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
    }
}

