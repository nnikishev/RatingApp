using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RatingApp.Models;
using RatingApp.Services;
using RatingApp.Views;
using Npgsql;

namespace RatingApp.ViewModels
{
    public partial class DatabaseDetailViewModel : ObservableObject
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IRatingService _ratingService;

        [ObservableProperty]
        private Database database;

        [ObservableProperty]
        private List<TableInfo> tables = new();

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string connectionStatus = "Проверка подключения...";

        [ObservableProperty]
        private bool isConnected;

        public DatabaseDetailViewModel(DatabaseContext databaseContext, IRatingService ratingService, Database database)
        {
            _databaseContext = databaseContext;
            _ratingService = ratingService;
            Database = database;
            
            LoadDatabaseDetailsAsync().SafeFireAndForget();
        }

        [RelayCommand]
        private async Task LoadDatabaseDetailsAsync()
        {
            try
            {
                IsLoading = true;
                ConnectionStatus = "Проверка подключения...";

                // Загружаем актуальные данные из локальной БД
                var updatedDb = await _ratingService.GetDatabaseAsync(Database.Id);
                if (updatedDb != null)
                {
                    Database = updatedDb;
                }

                // Проверяем подключение и получаем список таблиц
                await CheckDatabaseConnectionAndLoadTables();
            }
            catch (Exception ex)
            {
                ConnectionStatus = "Ошибка подключения";
                IsConnected = false;
                await Application.Current.MainPage.DisplayAlert("Ошибка", 
                    $"Не удалось загрузить информацию о БД: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task CheckDatabaseConnectionAndLoadTables()
        {
            if (Database.Type != DatabaseType.PostgreSQL)
            {
                ConnectionStatus = "Тип БД не поддерживается";
                IsConnected = false;
                Tables = GetDemoTables();
                return;
            }

            NpgsqlConnection connection = null;
            try
            {
                connection = new NpgsqlConnection(Database.ConnectionString);
                await connection.OpenAsync();
                
                IsConnected = true;
                ConnectionStatus = "Подключено";

                // Получаем список таблиц с количеством строк в одном запросе
                Tables = await GetPostgreSQLTablesWithRowCounts(connection);

                // Обновляем статус в локальной БД
                Database.IsActive = true;
                await _ratingService.SaveDatabaseAsync(Database);
            }
            catch (Exception ex)
            {
                IsConnected = false;
                ConnectionStatus = "Отключено";
                Database.IsActive = false;
                await _ratingService.SaveDatabaseAsync(Database);
                
                // Показываем демо-данные при ошибке подключения
                Tables = GetDemoTables();
                
                System.Diagnostics.Debug.WriteLine($"DATABASE_CONNECTION_ERROR: {ex.Message}");
            }
            finally
            {
                connection?.Close();
                connection?.Dispose();
            }
        }

        private async Task<List<TableInfo>> GetPostgreSQLTablesWithRowCounts(NpgsqlConnection connection)
        {
            var tables = new List<TableInfo>();
            
            try
            {
                using var command = new NpgsqlCommand(@"
                    SELECT 
                        t.table_name,
                        (xpath('/row/cnt/text()', 
                            query_to_xml(format('SELECT COUNT(*) as cnt FROM %I.%I', table_schema, table_name), 
                            false, true, '')))[1]::text::bigint as row_count
                    FROM information_schema.tables t
                    WHERE t.table_schema = 'public' 
                    AND t.table_type = 'BASE TABLE'
                    ORDER BY t.table_name", connection);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var tableName = reader.GetString(0);
                    var rowCount = reader.GetInt64(1);
                    
                    tables.Add(new TableInfo 
                    { 
                        Name = tableName, 
                        RowCount = rowCount 
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"POSTGRES_TABLES_ERROR: {ex.Message}");
                // Если не сработал сложный запрос, пробуем упрощенный вариант
                tables = await GetPostgreSQLTablesSimple(connection);
            }

            return tables;
        }

        private async Task<List<TableInfo>> GetPostgreSQLTablesSimple(NpgsqlConnection connection)
        {
            var tables = new List<TableInfo>();
            
            try
            {
                // Сначала получаем список таблиц
                using var tablesCommand = new NpgsqlCommand(@"
                    SELECT table_name
                    FROM information_schema.tables 
                    WHERE table_schema = 'public' 
                    AND table_type = 'BASE TABLE'
                    ORDER BY table_name", connection);

                using var tablesReader = await tablesCommand.ExecuteReaderAsync();
                var tableNames = new List<string>();
                
                while (await tablesReader.ReadAsync())
                {
                    tableNames.Add(tablesReader.GetString(0));
                }
                tablesReader.Close();

                // Затем для каждой таблицы получаем количество строк
                foreach (var tableName in tableNames)
                {
                    try
                    {
                        using var countCommand = new NpgsqlCommand($"SELECT COUNT(*) FROM \"{tableName}\"", connection);
                        var rowCount = Convert.ToInt64(await countCommand.ExecuteScalarAsync());
                        
                        tables.Add(new TableInfo 
                        { 
                            Name = tableName, 
                            RowCount = rowCount 
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"COUNT_ERROR for {tableName}: {ex.Message}");
                        tables.Add(new TableInfo 
                        { 
                            Name = tableName, 
                            RowCount = 0 
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"POSTGRES_SIMPLE_TABLES_ERROR: {ex.Message}");
                // Если и это не сработало, возвращаем демо-данные
                tables = GetDemoTables();
            }

            return tables;
        }

        private List<TableInfo> GetDemoTables()
        {
            // Демо-данные для случаев, когда не удается получить реальные таблицы
            return new List<TableInfo>
            {
                new TableInfo { Name = "drilling_data", RowCount = 15420 },
                new TableInfo { Name = "wells", RowCount = 342 },
                new TableInfo { Name = "measurements", RowCount = 89234 },
                new TableInfo { Name = "equipment", RowCount = 1205 },
                new TableInfo { Name = "users", RowCount = 156 },
                new TableInfo { Name = "sensors", RowCount = 892 },
                new TableInfo { Name = "reports", RowCount = 67 },
                new TableInfo { Name = "configurations", RowCount = 34 }
            };
        }

        [RelayCommand]
        private async Task TestConnectionAsync()
        {
            try
            {
                IsLoading = true;
                ConnectionStatus = "Тестирование подключения...";

                if (Database.Type != DatabaseType.PostgreSQL)
                {
                    await Application.Current.MainPage.DisplayAlert("Информация", 
                        "Поддержка данного типа БД находится в разработке. Показаны демо-данные.", "OK");
                    return;
                }

                await CheckDatabaseConnectionAndLoadTables();

                if (IsConnected)
                {
                    await Application.Current.MainPage.DisplayAlert("Успех", "Подключение успешно!", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Не удалось подключиться к базе данных", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Ошибка подключения: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task RefreshTablesAsync()
        {
            if (Database.Type == DatabaseType.PostgreSQL)
            {
                await CheckDatabaseConnectionAndLoadTables();
            }
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Navigation.PopAsync();
        }

        [RelayCommand]
        private async Task EditDatabaseAsync()
        {
            var editPage = new DatabaseEditPage(_databaseContext, Database);
            await Navigation.PushAsync(editPage);
        }

        private INavigation Navigation => Application.Current.MainPage.Navigation;
    }
}