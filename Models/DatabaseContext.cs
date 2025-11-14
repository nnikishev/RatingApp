using SQLite;
using RatingApp.Models;

namespace RatingApp.Models
{
    public class DatabaseContext
    {
        private SQLiteAsyncConnection _database;
        private bool _isInitialized = false;

        public DatabaseContext()
        {
            System.Diagnostics.Debug.WriteLine("DATABASE: Starting initialization");
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                if (_isInitialized) return;

                // Get database path
                var databasePath = Path.Combine(FileSystem.AppDataDirectory, "ratingapp.db");
                System.Diagnostics.Debug.WriteLine($"DATABASE_PATH: {databasePath}");

                // Check if directory exists
                var directory = Path.GetDirectoryName(databasePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    System.Diagnostics.Debug.WriteLine("DATABASE: Directory created");
                }

                // Create database connection
                _database = new SQLiteAsyncConnection(databasePath);
                System.Diagnostics.Debug.WriteLine("DATABASE: Connection created");

                // Create table synchronously
                var createTask = _database.CreateTableAsync<RatingItem>();
                createTask.Wait(); // Wait for completion
                
                if (createTask.Status == TaskStatus.RanToCompletion)
                {
                    System.Diagnostics.Debug.WriteLine("DATABASE: Table created successfully");
                    _isInitialized = true;
                    
                    // Add sample data
                    SeedDataAsync().SafeFireAndForget();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("DATABASE: Table creation failed");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DATABASE_INIT_ERROR: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"DATABASE_STACKTRACE: {ex.StackTrace}");
            }
        }

        private async Task SeedDataAsync()
        {
            try
            {
                var count = await _database.Table<RatingItem>().CountAsync();
                System.Diagnostics.Debug.WriteLine($"DATABASE_SEED: Current item count: {count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DATABASE_SEED_ERROR: {ex.Message}");
            }
        }

        public async Task<List<RatingItem>> GetItemsAsync()
        {
            try
            {
                if (!_isInitialized)
                {
                    System.Diagnostics.Debug.WriteLine("DATABASE_GET: Database not initialized, returning empty list");
                    return new List<RatingItem>();
                }

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

        // Method to check database status
        public async Task<bool> CheckDatabaseStatus()
        {
            try
            {
                if (!_isInitialized) return false;
                
                var count = await _database.Table<RatingItem>().CountAsync();
                System.Diagnostics.Debug.WriteLine($"DATABASE_STATUS: Is initialized: {_isInitialized}, Items count: {count}");
                return _isInitialized;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DATABASE_STATUS_ERROR: {ex.Message}");
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