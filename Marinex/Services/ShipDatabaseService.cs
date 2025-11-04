using Npgsql;
using Marinex.Models;

namespace Marinex.Services
{
    /// <summary>
    /// Implementasi IDatabaseService untuk operasi Ship
    /// Demonstrasi Polymorphism - implementasi berbeda dari interface yang sama
    /// </summary>
    public class ShipDatabaseService : IDatabaseService
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

        // Polymorphism: Implementasi interface method
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
                Console.WriteLine($"ShipDatabaseService connection error: {ex.Message}");
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
                Console.WriteLine($"ShipDatabaseService disconnect error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsConnectedAsync()
        {
            return _connection != null && _connection.State == System.Data.ConnectionState.Open;
        }

        // Polymorphism: Implementasi generic method dengan spesialisasi untuk Ship
        public async Task<T?> GetByIdAsync<T>(int id) where T : class
        {
            if (typeof(T) != typeof(Ship))
                throw new ArgumentException("This service only supports Ship type");

            try
            {
                if (!await IsConnectedAsync())
                    await ConnectAsync();

                // Schema: Tabel Ship dengan kolom ShipID, ShipName, ShipType, Owner, Capacity, Status
                string query = @"SELECT ""ShipID"", ""ShipName"", ""ShipType"", ""Owner"", ""Capacity"", ""Status"" 
                                FROM ""Ship"" WHERE ""ShipID"" = @id";
                
                using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("id", id);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Ship
                    {
                        ShipID = reader.GetInt32(0),
                        ShipName = reader.GetString(1),
                        ShipType = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Owner = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Capacity = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                        Status = reader.IsDBNull(5) ? null : reader.GetString(5)
                    } as T;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ShipDatabaseService GetById error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SaveAsync<T>(T entity) where T : class
        {
            if (typeof(T) != typeof(Ship) || entity is not Ship ship)
                throw new ArgumentException("This service only supports Ship type");

            try
            {
                if (!await IsConnectedAsync())
                    await ConnectAsync();

                // Schema: Tabel Ship dengan kolom ShipID, ShipName, ShipType, Owner, Capacity, Status
                string query = @"INSERT INTO ""Ship"" (""ShipName"", ""ShipType"", ""Owner"", ""Capacity"", ""Status"") 
                                VALUES (@shipname, @shiptype, @owner, @capacity, @status)
                                RETURNING ""ShipID""";

                using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("shipname", ship.ShipName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("shiptype", ship.ShipType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("owner", ship.Owner ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("capacity", ship.Capacity);
                cmd.Parameters.AddWithValue("status", ship.Status ?? (object)DBNull.Value);

                var result = await cmd.ExecuteScalarAsync();
                if (result != null)
                {
                    ship.ShipID = Convert.ToInt32(result);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ShipDatabaseService Save error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync<T>(int id) where T : class
        {
            try
            {
                if (!await IsConnectedAsync())
                    await ConnectAsync();

                // Schema: Tabel Ship
                string query = @"DELETE FROM ""Ship"" WHERE ""ShipID"" = @id";

                using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("id", id);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ShipDatabaseService Delete error: {ex.Message}");
                return false;
            }
        }
    }
}

