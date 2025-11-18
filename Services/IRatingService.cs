using RatingApp.Models;

namespace RatingApp.Services
{
    public interface IRatingService
    {
        Task<List<RatingItem>> GetItemsAsync();
        Task<RatingItem?> GetItemAsync(int id);
        Task<int> SaveItemAsync(RatingItem item);
        Task<int> DeleteItemAsync(RatingItem item);
        Task<bool> IsDatabaseReady();
        
        // Новые методы для работы с базами данных
        Task<List<Database>> GetDatabasesAsync();
        Task<Database?> GetDatabaseAsync(int id);
        Task<int> SaveDatabaseAsync(Database database);
        Task<int> DeleteDatabaseAsync(Database database);
        
        // Методы для работы с источниками
        Task<List<Dataset>> GetDatasetsAsync();
        Task<Dataset?> GetDatasetAsync(int id);
        Task<int> SaveDatasetAsync(Dataset dataset);
        Task<int> DeleteDatasetAsync(Dataset dataset);
        Task<List<Dataset>> GetDatasetsByDatabaseAsync(int databaseId);
    }
}