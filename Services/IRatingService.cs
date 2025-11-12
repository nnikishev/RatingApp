using RatingApp.Models;

namespace RatingApp.Services
{
    public interface IRatingService
    {
        Task<List<RatingItem>> GetItemsAsync();
        Task<RatingItem?> GetItemAsync(int id);
        Task<int> SaveItemAsync(RatingItem item);
        Task<int> DeleteItemAsync(RatingItem item);
        Task<bool> IsDatabaseReady(); // New method to check database status
    }
    
    public class RatingService : IRatingService
    {
        private readonly DatabaseContext _databaseContext;
        
        public RatingService()
        {
            _databaseContext = new DatabaseContext();
        }
        
        public Task<List<RatingItem>> GetItemsAsync() => _databaseContext.GetItemsAsync();
        public Task<RatingItem?> GetItemAsync(int id) => _databaseContext.GetItemAsync(id);
        public Task<int> SaveItemAsync(RatingItem item) => _databaseContext.SaveItemAsync(item);
        public Task<int> DeleteItemAsync(RatingItem item) => _databaseContext.DeleteItemAsync(item);
        
        public async Task<bool> IsDatabaseReady()
        {
            return await _databaseContext.CheckDatabaseStatus();
        }
    }
}