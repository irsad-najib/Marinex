namespace Marinex.Services
{
    /// <summary>
    /// Interface untuk Polymorphism demonstration
    /// Menggunakan konsep Polymorphism untuk berbagai implementasi database service
    /// </summary>
    public interface IDatabaseService
    {
        // Method yang harus diimplementasikan oleh semua class yang implement interface ini
        Task<bool> ConnectAsync();
        Task<bool> DisconnectAsync();
        Task<bool> IsConnectedAsync();
        
        // Method umum untuk operasi database
        Task<T?> GetByIdAsync<T>(int id) where T : class;
        Task<bool> SaveAsync<T>(T entity) where T : class;
        Task<bool> DeleteAsync<T>(int id) where T : class;
    }
}

