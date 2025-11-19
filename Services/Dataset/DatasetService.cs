using RatingApp.Models;

namespace RatingApp.Services
{
    public class DatasetService : IDatasetService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IRatingService _ratingService;

        public DatasetService(DatabaseContext databaseContext, IRatingService ratingService)
        {
            _databaseContext = databaseContext;
            _ratingService = ratingService;
        }

        public async Task<List<Dataset>> GetDatasetsAsync()
        {
            var datasets = await _databaseContext.GetDatasetsAsync();
            
            // Загружаем связанные базы данных
            foreach (var dataset in datasets)
            {
                if (dataset.DatabaseId > 0)
                {
                    dataset.Database = await _ratingService.GetDatabaseAsync(dataset.DatabaseId);
                }
            }
            
            return datasets;
        }

        public async Task<Dataset?> GetDatasetAsync(int id)
        {
            var dataset = await _databaseContext.GetDatasetAsync(id);
            if (dataset != null && dataset.DatabaseId > 0)
            {
                dataset.Database = await _ratingService.GetDatabaseAsync(dataset.DatabaseId);
            }
            return dataset;
        }

        public async Task<int> SaveDatasetAsync(Dataset dataset)
        {
            return await _databaseContext.SaveDatasetAsync(dataset);
        }

        public async Task<int> DeleteDatasetAsync(Dataset dataset)
        {
            return await _databaseContext.DeleteDatasetAsync(dataset);
        }
    }
}