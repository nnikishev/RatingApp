using RatingApp.ViewModels;
using RatingApp.Services;
using RatingApp.Models;

namespace RatingApp.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(IAuthService authService, IRatingService ratingService, DatabaseContext databaseContext)
        {
            InitializeComponent();
            BindingContext = new LoginViewModel(authService, ratingService, databaseContext);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            // Clear fields when page appears
            if (BindingContext is LoginViewModel viewModel)
            {
                viewModel.Username = string.Empty;
                viewModel.Password = string.Empty;
                viewModel.ErrorMessage = string.Empty;
                viewModel.IsLoginFailed = false;
            }
        }
    }
}