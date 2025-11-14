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
        Task<List<Source>> GetSourcesAsync();
        Task<List<Source>> GetSourcesByDatabaseIdAsync(int databaseId);
        Task<Source?> GetSourceAsync(int id);
        Task<int> SaveSourceAsync(Source source);
        Task<int> DeleteSourceAsync(Source source);
        
        // Методы для работы с диаграммами
        Task<List<Chart>> GetChartsAsync();
        Task<List<Chart>> GetChartsBySourceIdAsync(int sourceId);
        Task<Chart?> GetChartAsync(int id);
        Task<int> SaveChartAsync(Chart chart);
        Task<int> DeleteChartAsync(Chart chart);
        
        // Методы для работы с дашбордами
        Task<List<Dashboard>> GetDashboardsAsync();
        Task<Dashboard?> GetDashboardAsync(int id);
        Task<int> SaveDashboardAsync(Dashboard dashboard);
        Task<int> DeleteDashboardAsync(Dashboard dashboard);
    }
}