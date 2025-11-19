using RatingApp.Models;

namespace RatingApp.Services
{
    public interface IDatasetService
    {
        
        Task<List<Dataset>> GetDatasetsAsync();
        Task<Dataset?> GetDatasetAsync(int id);
        Task<int> SaveDatasetAsync(Dataset dataset);
        Task<int> DeleteDatasetAsync(Dataset dataset);
        // Task<List<Dataset>> GetDatasetsByDatabaseAsync(int databaseId);
    }
}