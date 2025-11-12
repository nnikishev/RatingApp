using RatingApp.ViewModels;
using RatingApp.Views;
using RatingApp.Services;
using SQLitePCL;

namespace RatingApp
{
    public partial class App : Application
    {
        public App(IRatingService ratingService)
        {
            // Initialize SQLite
            InitializeSQLite();
            
            InitializeComponent();

            // Global exception handlers
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var exception = (Exception)args.ExceptionObject;
                System.Diagnostics.Debug.WriteLine($"UNHANDLED_EXCEPTION: {exception}");
            };

            try
            {
                var mainPage = new MainPage(ratingService);
                MainPage = new NavigationPage(mainPage);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"APP_INIT_ERROR: {ex}");
                MainPage = new ContentPage
                {
                    Content = new Label { Text = "Application startup error" }
                };
            }
        }

        private void InitializeSQLite()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("SQLITE_INIT: Starting SQLite initialization");
                
                // Initialize SQLite
                Batteries_V2.Init();
                
                System.Diagnostics.Debug.WriteLine("SQLITE_INIT: SQLite initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQLITE_INIT_ERROR: {ex.Message}");
            }
        }
    }
}