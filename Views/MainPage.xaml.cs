using RatingApp.ViewModels;
using RatingApp.Services;

namespace RatingApp.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(IRatingService ratingService, IAuthService authService)
        {
            InitializeComponent();
            BindingContext = new MainViewModel(ratingService, authService);
        }
    }
}