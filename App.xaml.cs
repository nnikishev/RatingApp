using RatingApp.ViewModels;
using RatingApp.Views;
using RatingApp.Services;

namespace RatingApp
{
    public partial class App : Application
    {
        private readonly IAuthService _authService;
        private readonly IRatingService _ratingService;

        public App(IAuthService authService, IRatingService ratingService)
        {
            _authService = authService;
            _ratingService = ratingService;
            
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
                    MainPage = new NavigationPage(new MainPage(_ratingService, _authService));
                }
                else
                {
                    // User needs to login
                    System.Diagnostics.Debug.WriteLine("APP_START: User not authenticated, showing login");
                    MainPage = new LoginPage(_authService, _ratingService);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"APP_INIT_ERROR: {ex.Message}");
                // Fallback to login page
                MainPage = new LoginPage(_authService, _ratingService);
            }
        }
    }
}