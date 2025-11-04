using Npgsql;
using Marinex.Models;

namespace Marinex.Services
{
    /// <summary>
    /// Implementasi IDatabaseService untuk operasi Voyage
    /// Demonstrasi Polymorphism - implementasi berbeda dari interface yang sama dengan behavior berbeda
    /// </summary>
    public class VoyageDatabaseService : IDatabaseService
    {
        private static string GetConnectionString()
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = "db.zbwkuuzmhdaathgrsmff.supabase.co",
                Port = 5432,
                Database = "postgres",
                Username = "postgres",
                Password = "marinex123.",
                SslMode = SslMode.Prefer
            };
            return builder.ConnectionString;
        }
        
        private static string ConnectionString => GetConnectionString();
        private NpgsqlConnection? _connection;

        // Polymorphism: Implementasi interface method dengan behavior yang sama
        public async Task<bool> ConnectAsync()
        {
            try
            {
                _connection = new NpgsqlConnection(ConnectionString);
                await _connection.OpenAsync();
                return _connection.State == System.Data.ConnectionState.Open;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"VoyageDatabaseService connection error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DisconnectAsync()
        {
            try
            {
                if (_connection != null && _connection.State != System.Data.ConnectionState.Closed)
                {
                    await _connection.CloseAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"VoyageDatabaseService disconnect error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsConnectedAsync()
        {
            return _connection != null && _connection.State == System.Data.ConnectionState.Open;
        }

        // Polymorphism: Implementasi generic method dengan spesialisasi untuk Voyage (beda dengan ShipDatabaseService)
        public async Task<T?> GetByIdAsync<T>(int id) where T : class
        {
            if (typeof(T) != typeof(Voyage))
                throw new ArgumentException("This service only supports Voyage type");

            try
            {
                if (!await IsConnectedAsync())
                    await ConnectAsync();

                // Schema: Tabel Voyage dengan kolom VoyageID, "From", Destination, EstimatedDuration, ShipID, UserID
                // Note: "From" adalah reserved word di PostgreSQL, jadi perlu quote
                string query = @"SELECT ""VoyageID"", ""From"", ""Destination"", ""EstimatedDuration"", ""ShipID"", ""UserID"" 
                                FROM ""Voyage"" WHERE ""VoyageID"" = @id";

                using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("id", id);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Voyage
                    {
                        VoyageID = reader.GetInt32(0),
                        From = reader.IsDBNull(1) ? null : reader.GetString(1),
                        Destination = reader.IsDBNull(2) ? null : reader.GetString(2),
                        EstimatedDuration = reader.IsDBNull(3) ? TimeSpan.Zero : reader.GetTimeSpan(3),
                        ShipID = reader.GetInt32(4),
                        UserID = reader.GetInt32(5)
                    } as T;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"VoyageDatabaseService GetById error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SaveAsync<T>(T entity) where T : class
        {
            if (typeof(T) != typeof(Voyage) || entity is not Voyage voyage)
                throw new ArgumentException("This service only supports Voyage type");

            try
            {
                if (!await IsConnectedAsync())
                    await ConnectAsync();

                // Schema: Tabel Voyage dengan kolom "From" (reserved word, perlu quote)
                string query = @"INSERT INTO ""Voyage"" (""From"", ""Destination"", ""EstimatedDuration"", ""ShipID"", ""UserID"") 
                                VALUES (@from, @destination, @duration, @shipid, @userid)
                                RETURNING ""VoyageID""";

                using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("from", voyage.From ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("destination", voyage.Destination ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("duration", voyage.EstimatedDuration);
                cmd.Parameters.AddWithValue("shipid", voyage.ShipID);
                cmd.Parameters.AddWithValue("userid", voyage.UserID);

                var result = await cmd.ExecuteScalarAsync();
                if (result != null)
                {
                    voyage.VoyageID = Convert.ToInt32(result);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"VoyageDatabaseService Save error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync<T>(int id) where T : class
        {
            try
            {
                if (!await IsConnectedAsync())
                    await ConnectAsync();

                // Schema: Tabel Voyage
                string query = @"DELETE FROM ""Voyage"" WHERE ""VoyageID"" = @id";

                using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("id", id);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"VoyageDatabaseService Delete error: {ex.Message}");
                return false;
            }
        }

        // Method khusus untuk VoyageDatabaseService (tidak ada di ShipDatabaseService)
        // Ini juga bagian dari Polymorphism - setiap implementasi bisa punya method tambahan
        public async Task<List<Voyage>> GetVoyagesByShipAsync(int shipId)
        {
            try
            {
                if (!await IsConnectedAsync())
                    await ConnectAsync();

                // Schema: Tabel Voyage dengan kolom "From" (reserved word)
                string query = @"SELECT ""VoyageID"", ""From"", ""Destination"", ""EstimatedDuration"", ""ShipID"", ""UserID"" 
                                FROM ""Voyage"" WHERE ""ShipID"" = @shipid";

                using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("shipid", shipId);

                var voyages = new List<Voyage>();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    voyages.Add(new Voyage
                    {
                        VoyageID = reader.GetInt32(0),
                        From = reader.IsDBNull(1) ? null : reader.GetString(1),
                        Destination = reader.IsDBNull(2) ? null : reader.GetString(2),
                        EstimatedDuration = reader.IsDBNull(3) ? TimeSpan.Zero : reader.GetTimeSpan(3),
                        ShipID = reader.GetInt32(4),
                        UserID = reader.GetInt32(5)
                    });
                }
                return voyages;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"VoyageDatabaseService GetVoyagesByShip error: {ex.Message}");
                return new List<Voyage>();
            }
        }
    }
}

