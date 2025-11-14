using SQLite;
using System.Diagnostics;

namespace RatingApp.Models
{
    public class DatabaseContext
    {
        private SQLiteAsyncConnection _database;
        private bool _isInitialized = false;

        public DatabaseContext()
        {
            Debug.WriteLine("DATABASE: Starting universal initialization");
            InitializeDatabase();
        }
        public async Task<List<RatingItem>> GetItemsAsync()
        {
            try
            {
                if (!_isInitialized) return new List<RatingItem>();

                var items = await _database.Table<RatingItem>().OrderByDescending(x => x.CreatedDate).ToListAsync();
                System.Diagnostics.Debug.WriteLine($"DATABASE_GET: Retrieved {items.Count} items");
                return items;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DATABASE_GET_ERROR: {ex.Message}");
                return new List<RatingItem>();
            }
        }

        public async Task<RatingItem?> GetItemAsync(int id)
        {
            try
            {
                if (!_isInitialized) return null;

                return await _database.Table<RatingItem>().Where(i => i.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DATABASE_GET_ITEM_ERROR: {ex.Message}");
                return null;
            }
        }

        public async Task<int> SaveItemAsync(RatingItem item)
        {
            try
            {
                if (!_isInitialized) return 0;

                if (item.Id != 0)
                {
                    var result = await _database.UpdateAsync(item);
                    System.Diagnostics.Debug.WriteLine($"DATABASE_SAVE: Updated item {item.Name}");
                    return result;
                }
                else
                {
                    var result = await _database.InsertAsync(item);
                    System.Diagnostics.Debug.WriteLine($"DATABASE_SAVE: Inserted item {item.Name}");
                    return result;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DATABASE_SAVE_ERROR: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> DeleteItemAsync(RatingItem item)
        {
            try
            {
                if (!_isInitialized) return 0;

                var result = await _database.DeleteAsync(item);
                System.Diagnostics.Debug.WriteLine($"DATABASE_DELETE: Deleted item {item.Name}");
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DATABASE_DELETE_ERROR: {ex.Message}");
                return 0;
            }
        }
        private async void InitializeDatabase()
        {
            try
            {
                if (_isInitialized) return;

                var databasePath = Path.Combine(FileSystem.AppDataDirectory, "ratingapp.db");
                Debug.WriteLine($"DATABASE_PATH: {databasePath}");

                var directory = Path.GetDirectoryName(databasePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    Debug.WriteLine("DATABASE: Directory created");
                }

                _database = new SQLiteAsyncConnection(databasePath);
                Debug.WriteLine("DATABASE: Connection created");

                // Создаем все таблицы
                await _database.CreateTableAsync<Database>();
                await _database.CreateTableAsync<Source>();
                await _database.CreateTableAsync<Chart>();
                await _database.CreateTableAsync<Dashboard>();
                
                Debug.WriteLine("DATABASE: All tables created successfully");
                _isInitialized = true;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DATABASE_INIT_ERROR: {ex.Message}");
                Debug.WriteLine($"DATABASE_STACKTRACE: {ex.StackTrace}");
            }
        }


        // Универсальные методы для работы с любыми сущностями

        public async Task<List<T>> GetAllAsync<T>() where T : BaseEntity, new()
        {
            try
            {
                if (!_isInitialized) return new List<T>();

                var items = await _database.Table<T>().ToListAsync();
                Debug.WriteLine($"DATABASE_GET_ALL: Retrieved {items.Count} items of type {typeof(T).Name}");
                return items;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DATABASE_GET_ALL_ERROR: {ex.Message}");
                return new List<T>();
            }
        }

        public async Task<T> GetByIdAsync<T>(int id) where T : BaseEntity, new()
        {
            try
            {
                if (!_isInitialized) return default(T);

                return await _database.Table<T>().Where(x => x.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DATABASE_GET_BY_ID_ERROR: {ex.Message}");
                return default(T);
            }
        }

        public async Task<int> SaveAsync<T>(T entity) where T : BaseEntity
        {
            try
            {
                if (!_isInitialized) return 0;

                if (entity.Id != 0)
                {
                    var result = await _database.UpdateAsync(entity);
                    Debug.WriteLine($"DATABASE_SAVE: Updated {typeof(T).Name} with ID {entity.Id}");
                    return result;
                }
                else
                {
                    var result = await _database.InsertAsync(entity);
                    Debug.WriteLine($"DATABASE_SAVE: Inserted {typeof(T).Name} with ID {result}");
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DATABASE_SAVE_ERROR: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> DeleteAsync<T>(T entity) where T : BaseEntity
        {
            try
            {
                if (!_isInitialized) return 0;

                var result = await _database.DeleteAsync(entity);
                Debug.WriteLine($"DATABASE_DELETE: Deleted {typeof(T).Name} with ID {entity.Id}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DATABASE_DELETE_ERROR: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> DeleteByIdAsync<T>(int id) where T : BaseEntity, new()
        {
            try
            {
                if (!_isInitialized) return 0;

                var entity = await GetByIdAsync<T>(id);
                if (entity != null)
                {
                    return await DeleteAsync(entity);
                }
                return 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DATABASE_DELETE_BY_ID_ERROR: {ex.Message}");
                return 0;
            }
        }

        // Специфические методы для удобства

        public async Task<List<Source>> GetSourcesByDatabaseIdAsync(int databaseId)
        {
            try
            {
                if (!_isInitialized) return new List<Source>();

                return await _database.Table<Source>()
                    .Where(s => s.DatabaseId == databaseId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DATABASE_GET_SOURCES_ERROR: {ex.Message}");
                return new List<Source>();
            }
        }

        public async Task<List<Chart>> GetChartsBySourceIdAsync(int sourceId)
        {
            try
            {
                if (!_isInitialized) return new List<Chart>();

                return await _database.Table<Chart>()
                    .Where(c => c.SourceId == sourceId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DATABASE_GET_CHARTS_ERROR: {ex.Message}");
                return new List<Chart>();
            }
        }

        // Метод проверки статуса базы данных
        public async Task<bool> CheckDatabaseStatus()
        {
            try
            {
                if (!_isInitialized) return false;
                
                var count = await _database.Table<Database>().CountAsync();
                Debug.WriteLine($"DATABASE_STATUS: Is initialized: {_isInitialized}, Databases count: {count}");
                return _isInitialized;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DATABASE_STATUS_ERROR: {ex.Message}");
                return false;
            }
        }
    }

    public static class TaskExtensions
    {
        public static async void SafeFireAndForget(this Task task, bool continueOnCapturedContext = true, Action<Exception> onException = null)
        {
            try
            {
                await task.ConfigureAwait(continueOnCapturedContext);
            }
            catch (Exception ex) when (onException != null)
            {
                onException(ex);
            }
        }
    }

    
}