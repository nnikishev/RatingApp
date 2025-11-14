using RatingApp.ViewModels;
using RatingApp.Views;
using RatingApp.Services;
using RatingApp.Models;

namespace RatingApp
{
    public partial class App : Application
    {
        private readonly IAuthService _authService;
        private readonly IRatingService _ratingService;
        private readonly DatabaseContext _databaseContext;

        public App(IAuthService authService, IRatingService ratingService, DatabaseContext databaseContext)
        {
            _authService = authService;
            _ratingService = ratingService;
            _databaseContext = databaseContext;
            
            InitializeComponent();
            InitializeApp();
        }

        private async void InitializeApp()
        {
            try
            {
                // Check if user is already authenticated
                var isAuthenticated = await _authService.ValidateTokenAsync();
                
                if (isAuthenticated)
                {
                    // User is authenticated, go to main app
                    var authInfo = _authService.GetAuthInfo();
                    System.Diagnostics.Debug.WriteLine($"APP_START: User {authInfo.Username} is authenticated");
                    MainPage = new NavigationPage(new MainPage(_ratingService, _authService, _databaseContext));
                }
                else
                {
                    // User needs to login
                    System.Diagnostics.Debug.WriteLine("APP_START: User not authenticated, showing login");
                    MainPage = new NavigationPage(new LoginPage(_authService, _ratingService, _databaseContext));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"APP_INIT_ERROR: {ex.Message}");
                // Fallback to login page
                MainPage = new NavigationPage(new LoginPage(_authService, _ratingService, _databaseContext));
            }
        }
    }
}