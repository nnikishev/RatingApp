using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RatingApp.Services;
using RatingApp.Views;

namespace RatingApp.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly IRatingService _ratingService;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isLoginFailed = false;

        public LoginViewModel(IAuthService authService, IRatingService ratingService)
        {
            _authService = authService;
            _ratingService = ratingService;
            Title = "Login";
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;
                IsLoginFailed = false;

                // Validation
                if (string.IsNullOrWhiteSpace(Username))
                {
                    ErrorMessage = "Please enter username";
                    IsLoginFailed = true;
                    return;
                }

                if (string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Please enter password";
                    IsLoginFailed = true;
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"LOGIN_ATTEMPT: Trying to login as {Username}");

                var loginResult = await _authService.LoginAsync(Username, Password);

                if (loginResult != null && !string.IsNullOrEmpty(loginResult.Token))
                {
                    System.Diagnostics.Debug.WriteLine($"LOGIN_SUCCESS: User {Username} logged in successfully");
                    
                    // Navigate to main app
                    Application.Current.MainPage = new NavigationPage(new MainPage(_ratingService, _authService));
                }
                else
                {
                    ErrorMessage = "Invalid username or password";
                    IsLoginFailed = true;
                    System.Diagnostics.Debug.WriteLine($"LOGIN_FAILED: Invalid credentials for {Username}");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Login failed. Please check your connection.";
                IsLoginFailed = true;
                System.Diagnostics.Debug.WriteLine($"LOGIN_EXCEPTION: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void ClearError()
        {
            ErrorMessage = string.Empty;
            IsLoginFailed = false;
        }
    }
}