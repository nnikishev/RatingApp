using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RatingApp.Services;
using RatingApp.Views;

namespace RatingApp.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly IRatingService _ratingService;
        private readonly IAuthService _authService;

        public MainViewModel(IRatingService ratingService, IAuthService authService)
        {
            _ratingService = ratingService;
            _authService = authService;
        }


        [RelayCommand]
        private async Task LogoutAsync()
        {
            try
            {
                bool answer = await Application.Current.MainPage.DisplayAlert(
                    "Logout",
                    "Are you sure you want to logout?",
                    "Yes", "No");

                if (answer)
                {
                    await _authService.LogoutAsync();

                    // Navigate back to login page
                    Application.Current.MainPage = new LoginPage(_authService, _ratingService);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Logout failed", "OK");
                System.Diagnostics.Debug.WriteLine($"LOGOUT_ERROR: {ex.Message}");
            }
        }
        public MainViewModel(IRatingService ratingService)
        {
            _ratingService = ratingService;
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
                await Application.Current.MainPage.DisplayAlert("Error",
                    $"Failed to open list: {ex.Message}", "OK");
            }
        }
        
        [RelayCommand]
        private async Task DatabasesListAsync()
        {
            try
            {
                var listDatabases = new Views.DatabasesViews.DatabasesListPage(_authService);
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
                await Application.Current.MainPage.DisplayAlert("Error", 
                    $"Failed to open list: {ex.Message}", "OK");
            }
        }



        [RelayCommand]
        private void ExitApp()
        {
            Application.Current?.Quit();
        }
    }
}