using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RatingApp.Models;
using RatingApp.Services;

namespace RatingApp.ViewModels
{
    public partial class DatabaseEditViewModel : ObservableObject
    {
        private readonly DatabaseContext _databaseContext;
        private readonly Database _originalDatabase;

        [ObservableProperty]
        private string host;

        [ObservableProperty]
        private string port;

        [ObservableProperty]
        private string user;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string databaseName;

        [ObservableProperty]
        private DatabaseType selectedType;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        private bool isTesting;

        [ObservableProperty]
        private bool isConnectionSuccessful;

        [ObservableProperty]
        private bool isEditMode;

        public List<DatabaseType> DatabaseTypes { get; } = Enum.GetValues<DatabaseType>().ToList();

        public DatabaseEditViewModel(DatabaseContext databaseContext, Database database = null)
        {
            _databaseContext = databaseContext;
            
            if (database != null)
            {
                _originalDatabase = database;
                IsEditMode = true;
                LoadDatabaseData(database);
            }
            else
            {
                // Значения по умолчанию для новой БД
                host = "localhost";
                port = "5432";
                selectedType = DatabaseType.PostgreSQL;
                isConnectionSuccessful = false;
            }
        }

        private void LoadDatabaseData(Database database)
        {
            Host = database.Host;
            Port = database.Port;
            User = database.User;
            Password = database.Password;
            DatabaseName = database.Name;
            SelectedType = database.Type;
            Description = database.Description;
            IsConnectionSuccessful = database.IsActive;
        }

        [RelayCommand]
        private async Task TestConnectionAsync()
        {
            try
            {
                IsTesting = true;
                IsConnectionSuccessful = false;

                // Имитация тестирования подключения
                await Task.Delay(2000);

                // Здесь будет реальная логика тестирования подключения к БД
                bool connectionSuccess = await SimulateConnectionTest();

                IsConnectionSuccessful = connectionSuccess;

                if (connectionSuccess)
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
                IsTesting = false;
            }
        }

        private async Task<bool> SimulateConnectionTest()
        {
            // Заглушка для тестирования подключения
            await Task.Delay(1000);
            
            // Имитация успешного подключения при заполненных полях
            return !string.IsNullOrWhiteSpace(Host) && 
                   !string.IsNullOrWhiteSpace(Port) && 
                   !string.IsNullOrWhiteSpace(User) && 
                   !string.IsNullOrWhiteSpace(DatabaseName);
        }

        [RelayCommand]
        private async Task SaveDatabaseAsync()
        {
            try
            {
                if (!ValidateInput())
                    return;

                var database = _originalDatabase ?? new Database();
                
                database.Host = Host;
                database.Port = Port;
                database.User = User;
                database.Password = Password;
                database.Name = DatabaseName;
                database.Type = SelectedType;
                database.Description = Description;
                database.IsActive = IsConnectionSuccessful;

                var result = await _databaseContext.SaveAsync(database);

                if (result > 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Успех", 
                        IsEditMode ? "База данных обновлена!" : "База данных создана!", "OK");
                    
                    // Возвращаемся назад после сохранения
                    await Navigation.PopAsync();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Не удалось сохранить базу данных", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Ошибка сохранения: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Navigation.PopAsync();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(Host))
            {
                Application.Current.MainPage.DisplayAlert("Ошибка", "Введите хост", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Port))
            {
                Application.Current.MainPage.DisplayAlert("Ошибка", "Введите порт", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(User))
            {
                Application.Current.MainPage.DisplayAlert("Ошибка", "Введите пользователя", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(DatabaseName))
            {
                Application.Current.MainPage.DisplayAlert("Ошибка", "Введите имя базы данных", "OK");
                return false;
            }

            return true;
        }

        // Свойство для доступа к навигации
        private INavigation Navigation => Application.Current.MainPage.Navigation;
    }
}