using RatingApp.Models;

namespace RatingApp.Services
{
    public class RatingService : IRatingService
    {
        private readonly DatabaseContext _databaseContext;

        public RatingService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        // Старые методы для RatingItem
        public async Task<List<RatingItem>> GetItemsAsync()
        {
            try
            {
                return await _databaseContext.GetAllAsync<RatingItem>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_GET_ITEMS_ERROR: {ex.Message}");
                return new List<RatingItem>();
            }
        }

        public async Task<RatingItem?> GetItemAsync(int id)
        {
            try
            {
                return await _databaseContext.GetByIdAsync<RatingItem>(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_GET_ITEM_ERROR: {ex.Message}");
                return null;
            }
        }

        public async Task<int> SaveItemAsync(RatingItem item)
        {
            try
            {
                return await _databaseContext.SaveAsync(item);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_SAVE_ITEM_ERROR: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> DeleteItemAsync(RatingItem item)
        {
            try
            {
                return await _databaseContext.DeleteAsync(item);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_DELETE_ITEM_ERROR: {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> IsDatabaseReady()
        {
            return await _databaseContext.CheckDatabaseStatus();
        }

        // Новые методы для работы с Database
        public async Task<List<Database>> GetDatabasesAsync()
        {
            try
            {
                return await _databaseContext.GetAllAsync<Database>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_GET_DATABASES_ERROR: {ex.Message}");
                return new List<Database>();
            }
        }

        public async Task<Database?> GetDatabaseAsync(int id)
        {
            try
            {
                return await _databaseContext.GetByIdAsync<Database>(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_GET_DATABASE_ERROR: {ex.Message}");
                return null;
            }
        }

        public async Task<int> SaveDatabaseAsync(Database database)
        {
            try
            {
                return await _databaseContext.SaveAsync(database);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_SAVE_DATABASE_ERROR: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> DeleteDatabaseAsync(Database database)
        {
            try
            {
                return await _databaseContext.DeleteAsync(database);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_DELETE_DATABASE_ERROR: {ex.Message}");
                return 0;
            }
        }

        // Методы для работы с Source
        public async Task<List<Source>> GetSourcesAsync()
        {
            try
            {
                return await _databaseContext.GetAllAsync<Source>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_GET_SOURCES_ERROR: {ex.Message}");
                return new List<Source>();
            }
        }

        public async Task<List<Source>> GetSourcesByDatabaseIdAsync(int databaseId)
        {
            try
            {
                return await _databaseContext.GetSourcesByDatabaseIdAsync(databaseId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_GET_SOURCES_BY_DB_ERROR: {ex.Message}");
                return new List<Source>();
            }
        }

        public async Task<Source?> GetSourceAsync(int id)
        {
            try
            {
                return await _databaseContext.GetByIdAsync<Source>(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_GET_SOURCE_ERROR: {ex.Message}");
                return null;
            }
        }

        public async Task<int> SaveSourceAsync(Source source)
        {
            try
            {
                return await _databaseContext.SaveAsync(source);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_SAVE_SOURCE_ERROR: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> DeleteSourceAsync(Source source)
        {
            try
            {
                return await _databaseContext.DeleteAsync(source);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_DELETE_SOURCE_ERROR: {ex.Message}");
                return 0;
            }
        }

        // Методы для работы с Chart
        public async Task<List<Chart>> GetChartsAsync()
        {
            try
            {
                return await _databaseContext.GetAllAsync<Chart>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_GET_CHARTS_ERROR: {ex.Message}");
                return new List<Chart>();
            }
        }

        public async Task<List<Chart>> GetChartsBySourceIdAsync(int sourceId)
        {
            try
            {
                return await _databaseContext.GetChartsBySourceIdAsync(sourceId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_GET_CHARTS_BY_SOURCE_ERROR: {ex.Message}");
                return new List<Chart>();
            }
        }

        public async Task<Chart?> GetChartAsync(int id)
        {
            try
            {
                return await _databaseContext.GetByIdAsync<Chart>(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_GET_CHART_ERROR: {ex.Message}");
                return null;
            }
        }

        public async Task<int> SaveChartAsync(Chart chart)
        {
            try
            {
                return await _databaseContext.SaveAsync(chart);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_SAVE_CHART_ERROR: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> DeleteChartAsync(Chart chart)
        {
            try
            {
                return await _databaseContext.DeleteAsync(chart);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_DELETE_CHART_ERROR: {ex.Message}");
                return 0;
            }
        }

        // Методы для работы с Dashboard
        public async Task<List<Dashboard>> GetDashboardsAsync()
        {
            try
            {
                return await _databaseContext.GetAllAsync<Dashboard>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_GET_DASHBOARDS_ERROR: {ex.Message}");
                return new List<Dashboard>();
            }
        }

        public async Task<Dashboard?> GetDashboardAsync(int id)
        {
            try
            {
                return await _databaseContext.GetByIdAsync<Dashboard>(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_GET_DASHBOARD_ERROR: {ex.Message}");
                return null;
            }
        }

        public async Task<int> SaveDashboardAsync(Dashboard dashboard)
        {
            try
            {
                return await _databaseContext.SaveAsync(dashboard);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_SAVE_DASHBOARD_ERROR: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> DeleteDashboardAsync(Dashboard dashboard)
        {
            try
            {
                return await _databaseContext.DeleteAsync(dashboard);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RATING_SERVICE_DELETE_DASHBOARD_ERROR: {ex.Message}");
                return 0;
            }
        }
    }
}