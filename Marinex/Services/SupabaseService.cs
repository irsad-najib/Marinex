using System;
using Npgsql;
using BCrypt.Net;

namespace Marinex.Services
{
    public class SupabaseService
    {
        private static string GetConnectionString()
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
        
        private static string ConnectionString => GetConnectionString();

        public async Task<bool> AuthenticateUser(string userName, string password)
        {
            try
            {
                using var conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                string query = @"SELECT password FROM ""User"" WHERE username = @userName LIMIT 1";
                
                using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("userName", userName);

                var result = await cmd.ExecuteScalarAsync();
                
                if (result == null)
                    return false;

                string hashedPassword = result.ToString()!;
                
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

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                string query = @"INSERT INTO ""User"" (username, password, role, company) 
                                VALUES (@userName, @password, 'User', @company)";
                
                using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("userName", userName);
                cmd.Parameters.AddWithValue("password", hashedPassword); 
                cmd.Parameters.AddWithValue("company", company);

                await cmd.ExecuteNonQueryAsync();
                return (true, string.Empty);
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "23505") 
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
