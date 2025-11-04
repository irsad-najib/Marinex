using System;
using Npgsql;
using BCrypt.Net;

namespace Marinex.Services
{
    public class SupabaseService
    {
        // IMPORTANT: Connection string sesuai dengan Supabase Dashboard
        // Dari Supabase: User Id=postgres.lybaccsnnivbycyyvrka;Server=aws-1-ap-southeast-2.pooler.supabase.com;Port=5432
        private static string GetConnectionString()
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = "aws-1-ap-southeast-2.pooler.supabase.com",
                Port = 5432,  // Port sesuai yang diberikan Supabase
                Database = "postgres",
                Username = "postgres.lybaccsnnivbycyyvrka",  // Username sesuai Supabase (ada 'l' di depan)
                Password = "marinex123.",  // Ganti dengan password Supabase Anda yang sebenarnya
                SslMode = SslMode.Require
            };
            return builder.ConnectionString;
        }
        
        private static string ConnectionString => GetConnectionString();

        public async Task<bool> AuthenticateUser(string userName, string password)
        {
            try
            {
                using var conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();

                // Get the hashed password from database
                // Schema: Tabel "User" dengan kolom lowercase (PostgreSQL akan lowercase-kan tanpa quote)
                string query = @"SELECT password FROM ""User"" WHERE username = @userName LIMIT 1";
                
                using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("userName", userName);

                var result = await cmd.ExecuteScalarAsync();
                
                if (result == null)
                    return false;

                string hashedPassword = result.ToString()!;
                
                // Verify the password using BCrypt
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Authentication error: {ex.Message}");
                return false;
            }
        }

        public async Task<(bool Success, string ErrorMessage)> RegisterUser(string userName, string password, string company)
        {
            try
            {
                using var conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();

                // Hash the password using BCrypt
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                // Insert new user with hashed password
                // Schema: Tabel "User" dengan kolom lowercase (PostgreSQL akan lowercase-kan tanpa quote)
                string query = @"INSERT INTO ""User"" (username, password, role, company) 
                                VALUES (@userName, @password, 'User', @company)";
                
                using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("userName", userName);
                cmd.Parameters.AddWithValue("password", hashedPassword); // Stored as hashed password
                cmd.Parameters.AddWithValue("company", company);

                await cmd.ExecuteNonQueryAsync();
                return (true, string.Empty);
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "23505") // Unique violation
            {
                string errorMsg = "Username sudah terdaftar. Silakan gunakan username lain atau login dengan username yang sudah terdaftar.";
                Console.WriteLine($"Registration error: {ex.Message}");
                return (false, errorMsg);
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "XX000")
            {
                string errorMsg = $"Koneksi database gagal: Tenant atau user tidak ditemukan.\n\n" +
                                $"Kemungkinan masalah:\n" +
                                $"1. Connection string salah (cek Host, Username, Password)\n" +
                                $"2. Project ID Supabase tidak cocok\n" +
                                $"3. Username/password salah\n\n" +
                                $"Detail Error: {ex.Message}\n" +
                                $"SQL State: {ex.SqlState}";
                Console.WriteLine($"Registration error (XX000): {ex.Message}\nSQL State: {ex.SqlState}");
                return (false, errorMsg);
            }
            catch (Npgsql.PostgresException ex)
            {
                string errorMsg = $"Database error: {ex.Message}\nSQL State: {ex.SqlState}";
                Console.WriteLine($"Registration error: {ex.Message}\nSQL State: {ex.SqlState}");
                return (false, errorMsg);
            }
            catch (Npgsql.NpgsqlException ex)
            {
                // Error "No such host is known" berarti host tidak ditemukan atau DNS tidak resolve
                string errorMsg = $"Koneksi database gagal: {ex.Message}\n\n" +
                                $"Kemungkinan masalah:\n" +
                                $"1. Host tidak ditemukan - cek apakah host benar: aws-1-ap-southeast-2.pooler.supabase.com\n" +
                                $"2. Koneksi internet tidak aktif\n" +
                                $"3. DNS tidak bisa resolve hostname\n" +
                                $"4. Firewall atau VPN memblokir koneksi\n\n" +
                                $"Pastikan:\n" +
                                $"- Host: aws-1-ap-southeast-2.pooler.supabase.com\n" +
                                $"- Username: postgres.lybaccsnnivbycyyvrka\n" +
                                $"- Password: marinex123. (atau password Supabase Anda yang benar)";
                Console.WriteLine($"Connection error: {ex.Message}\nInner: {ex.InnerException?.Message}");
                return (false, errorMsg);
            }
            catch (Exception ex)
            {
                string errorMsg = $"Terjadi kesalahan: {ex.Message}";
                Console.WriteLine($"Registration error: {ex.Message}");
                return (false, errorMsg);
            }
        }

        public async Task<bool> TestConnection()
        {
            try
            {
                using var conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                return conn.State == System.Data.ConnectionState.Open;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection test failed: {ex.Message}");
                return false;
            }
        }
    }
}
