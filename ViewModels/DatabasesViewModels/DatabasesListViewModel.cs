using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RatingApp.Models;
using RatingApp.Services;
using RatingApp.Views;
using Npgsql;

namespace RatingApp.ViewModels
{
    public partial class DatabasesListViewModel : ObservableObject
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IRatingService _ratingService;
        private readonly IDatasetService _datasetService;

        [ObservableProperty]
        private List<Database> databases;

        public DatabasesListViewModel(DatabaseContext databaseContext, IRatingService ratingService, IDatasetService datasetService)
        {
            _ratingService = ratingService;
            _databaseContext = databaseContext;
            _datasetService = datasetService;
            LoadDatabasesAsync().SafeFireAndForget();
        }

        [RelayCommand]
        private async Task LoadDatabasesAsync()
        {
            try
            {
                Databases = await _databaseContext.GetAllAsync<Database>();
                
                // Загружаем количество источников для каждой БД
                foreach (var db in Databases)
                {
                    var Datasets = await _databaseContext.GetDatasetsByDatabaseIdAsync(db.Id);
                    db.DatasetsCount = Datasets?.Count ?? 0;
                }
                
                OnPropertyChanged(nameof(Databases));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось загрузить базы данных: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task AddDatabaseAsync()
        {
            try
            {
                var editPage = new DatabaseEditPage(_databaseContext);
                await Navigation.PushAsync(editPage);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось открыть страницу создания: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task EditDatabaseAsync(Database database)
        {
            if (database != null)
            {
                try
                {
                    var editPage = new DatabaseEditPage(_databaseContext, database);
                    await Navigation.PushAsync(editPage);
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось открыть страницу редактирования: {ex.Message}", "OK");
                }
            }
        }

        [RelayCommand]
        private async Task OpenSqlShellAsync(Database database)
        {
            if (database == null) return;

            try
            {
                // Проверяем подключение перед открытием SQL Shell
                if (!await TestDatabaseConnection(database))
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Ошибка подключения", 
                        "Не удалось подключиться к базе данных", 
                        "OK");
                    return;
                }

                // Открываем страницу SQL Shell
                var sqlShellPage = new SqlShellPage(database, _datasetService);
                await Navigation.PushAsync(sqlShellPage);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Ошибка", 
                    $"Не удалось открыть SQL Shell: {ex.Message}", 
                    "OK");
            }
        }

        private async Task<bool> TestDatabaseConnection(Database database)
        {
            if (database.Type != DatabaseType.PostgreSQL) 
                return false; // Пока поддерживаем только PostgreSQL

            NpgsqlConnection connection = null;
            try
            {
                connection = new NpgsqlConnection(database.ConnectionString);
                await connection.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                connection?.Close();
                connection?.Dispose();
            }
        }

        [RelayCommand]
        private async Task DeleteDatabaseAsync(Database database)
        {
            if (database == null) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Удаление базы данных",
                $"Вы уверены, что хотите удалить базу данных '{database.Name}'?",
                "Удалить", "Отмена");

            if (confirm)
            {
                try
                {
                    // Удаляем связанные источники сначала
                    var Datasets = await _databaseContext.GetDatasetsByDatabaseIdAsync(database.Id);
                    foreach (var Dataset in Datasets)
                    {
                        await _databaseContext.DeleteAsync(Dataset);
                    }

                    // Удаляем саму базу данных
                    var result = await _databaseContext.DeleteAsync(database);
                    
                    if (result > 0)
                    {
                        await Application.Current.MainPage.DisplayAlert("Успех", "База данных удалена", "OK");
                        await LoadDatabasesAsync(); // Обновляем список
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось удалить базу данных: {ex.Message}", "OK");
                }
            }
        }

        [RelayCommand]
        private async Task ShowDatabaseDetailAsync(Database database)
        {
            if (database != null)
            {
                try
                {
                    var detailPage = new DatabaseDetailPage(_databaseContext, _ratingService, database);
                    await Navigation.PushAsync(detailPage);
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", 
                        $"Не удалось открыть детальную информацию: {ex.Message}", "OK");
                }
            }
        }


        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Navigation.PopAsync();
        }

        // Свойство для доступа к навигации
        private INavigation Navigation => Application.Current.MainPage.Navigation;
    }
}