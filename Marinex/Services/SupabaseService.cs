using System;
using Npgsql;
using BCrypt.Net;

namespace Marinex.Services
{
    public class SupabaseService
    {
        // IMPORTANT: Replace with your actual Supabase connection string
        // Format: postgresql://postgres:[YOUR-PASSWORD]@db.zbwkuuzmhdaathgrsmff.supabase.co:5432/postgres
        private const string ConnectionString = "postgresql://postgres:marinex123.@db.zbwkuuzmhdaathgrsmff.supabase.co:5432/postgres";

        public async Task<bool> AuthenticateUser(string email, string password)
        {
            try
            {
                using var conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();

                // Get the hashed password from database
                string query = "SELECT password FROM users WHERE email = @email LIMIT 1";
                
                using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("email", email);

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

        public async Task<bool> RegisterUser(string email, string password, string userName, string company)
        {
            try
            {
                using var conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();

                // Hash the password using BCrypt
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                // Insert new user with hashed password
                string query = @"INSERT INTO users (email, password, username, company, role) 
                                VALUES (@email, @password, @username, @company, 'User')";
                
                using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("email", email);
                cmd.Parameters.AddWithValue("password", hashedPassword); // Stored as hashed password
                cmd.Parameters.AddWithValue("username", userName);
                cmd.Parameters.AddWithValue("company", company);

                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration error: {ex.Message}");
                return false;
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
