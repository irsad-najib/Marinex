namespace Marinex.Services
{
    public interface IDatabaseService
    {
        Task<bool> ConnectAsync();
        Task<bool> DisconnectAsync();
        Task<bool> IsConnectedAsync();
        
        Task<T?> GetByIdAsync<T>(int id) where T : class;
        Task<bool> SaveAsync<T>(T entity) where T : class;
        Task<bool> DeleteAsync<T>(int id) where T : class;
    }
}

