using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RatingApp.Services;
using RatingApp.Views;
using RatingApp.Models;

namespace RatingApp.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly IRatingService _ratingService;
        private readonly IAuthService _authService;
        private readonly DatabaseContext _databaseContext;
        private readonly IDatasetService _datasetService;

        public MainViewModel(
            IRatingService ratingService, 
            IAuthService authService, 
            DatabaseContext databaseContext, 
            IDatasetService datasetService
            )
        {
            _ratingService = ratingService;
            _authService = authService;
            _databaseContext = databaseContext;
            _datasetService = datasetService;
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            try
            {
                bool answer = await Application.Current.MainPage.DisplayAlert(
                    "Выход",
                    "Вы уверены, что хотите выйти?",
                    "Да", "Нет");

                if (answer)
                {
                    await _authService.LogoutAsync();

                    // Navigate back to login page
                    Application.Current.MainPage = new NavigationPage(new LoginPage(_authService, _ratingService, _databaseContext, _datasetService));
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Не удалось выйти", "OK");
                System.Diagnostics.Debug.WriteLine($"LOGOUT_ERROR: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task ShowRatingAsync()
        {
            try
            {
                // Use RatingListPage with service injection
                var listPage = new RatingListPage(_ratingService);
                if (Application.Current?.MainPage is NavigationPage navPage)
                {
                    await navPage.PushAsync(listPage);
                }
                else
                {
                    Application.Current.MainPage = new NavigationPage(listPage);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка",
                    $"Не удалось открыть список рейтингов: {ex.Message}", "OK");
            }
        }
        
        [RelayCommand]
        private async Task DatabasesListAsync()
        {
            try
            {
                var listDatabases = new DatabasesListPage(_databaseContext, _ratingService, _datasetService);
                if (Application.Current?.MainPage is NavigationPage navPage)
                {
                    await navPage.PushAsync(listDatabases);
                }
                else
                {
                    Application.Current.MainPage = new NavigationPage(listDatabases);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", 
                    $"Не удалось открыть список баз данных: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task VisualizationsAsync()
        {
            try
            {
                // Здесь будет переход к визуализациям
                await Application.Current.MainPage.DisplayAlert("Информация", 
                    "Раздел визуализаций в разработке", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", 
                    $"Не удалось открыть визуализации: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task NavigateToDatasetsAsync()
        {
            var datasetsPage = new DatasetsListPage(
                new DatasetsListViewModel(
                    _datasetService,
                    _ratingService));

            await Application.Current.MainPage.Navigation.PushAsync(datasetsPage);
        }

        [RelayCommand]
        private void ExitApp()
        {
            try
            {
                bool answer = Application.Current.MainPage.DisplayAlert(
                    "Выход",
                    "Вы уверены, что хотите выйти из приложения?",
                    "Да", "Нет").GetAwaiter().GetResult();

                if (answer)
                {
                    Application.Current?.Quit();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXIT_APP_ERROR: {ex.Message}");
                Application.Current?.Quit();
            }
        }

        [RelayCommand]
        private async Task ProfileAsync()
        {
            try
            {
                await Application.Current.MainPage.DisplayAlert("Профиль", 
                    "Функционал профиля в разработке", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", 
                    $"Не удалось открыть профиль: {ex.Message}", "OK");
            }
        }
    }
}