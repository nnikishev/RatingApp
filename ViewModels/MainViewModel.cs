using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RatingApp.Services;
using RatingApp.Views;

namespace RatingApp.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly IRatingService _ratingService;

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
        private void ExitApp()
        {
            Application.Current?.Quit();
        }
    }
}