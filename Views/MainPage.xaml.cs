using RatingApp.ViewModels;
using RatingApp.Services;
using RatingApp.Models;

namespace RatingApp.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(IRatingService ratingService, IAuthService authService, DatabaseContext databaseContext)
        {
            InitializeComponent();
            BindingContext = new MainViewModel(ratingService, authService, databaseContext);
        }
    }
}