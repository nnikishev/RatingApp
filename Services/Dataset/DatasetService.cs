using RatingApp.Models;
using SQLite;

namespace RatingApp.Services
{
    public class DatasetService : IDatasetService
    {
        private SQLiteAsyncConnection _database;

        public DatasetService()
        {
            _database = DatabaseContext.GetConnection();
            InitializeAsync().SafeFireAndForget(false);
        }

        private async Task InitializeAsync()
        {
            await _database.CreateTableAsync<Dataset>();
        }

        public async Task<List<Dataset>> GetDatasetsAsync()
        {
            return await _database.Table<Dataset>()
                .OrderByDescending(d => d.UpdatedAt)
                .ToListAsync();
        }

        public async Task<Dataset?> GetDatasetAsync(int id)
        {
            return await _database.Table<Dataset>()
                .Where(d => d.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> SaveDatasetAsync(Dataset dataset)
        {
            dataset.UpdatedAt = DateTime.Now;
            
            if (dataset.Id == 0)
            {
                dataset.CreatedAt = DateTime.Now;
                return await _database.InsertAsync(dataset);
            }
            else
            {
                return await _database.UpdateAsync(dataset);
            }
        }

        public async Task<int> DeleteDatasetAsync(Dataset dataset)
        {
            return await _database.DeleteAsync(dataset);
        }
    }
}