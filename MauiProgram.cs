using RatingApp.Services;
using RatingApp.ViewModels;
using RatingApp.Views;
using RatingApp.Models;

namespace RatingApp
    {
        public static class MauiProgram
        {
            public static MauiApp CreateMauiApp()
            {
                var builder = MauiApp.CreateBuilder();
                builder
                    .UseMauiApp<App>()
                    .ConfigureFonts(fonts =>
                    {
                        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    });

                // Register services
                builder.Services.AddSingleton<DatabaseContext>();
                builder.Services.AddSingleton<IAuthService, AuthService>();
                builder.Services.AddSingleton<IRatingService, RatingService>();
                // Register converters if needed
                builder.Services.AddSingleton<Converters.RatingToColorConverter>();
                builder.Services.AddSingleton<Converters.EditSaveConverter>();
                builder.Services.AddSingleton<Converters.RatingVisibleConverter>();
                builder.Services.AddSingleton<MainViewModel>();

                // Register ViewModels
                builder.Services.AddTransient<MainViewModel>();
                builder.Services.AddTransient<LoginViewModel>();
                builder.Services.AddTransient<RatingListViewModel>();
                builder.Services.AddTransient<RatingFormViewModel>();
                builder.Services.AddTransient<RatingDetailViewModel>();
                builder.Services.AddTransient<DatabasesListViewModel>();
                builder.Services.AddTransient<DatabaseEditViewModel>();
                // Register Views
                builder.Services.AddTransient<MainPage>();
                builder.Services.AddTransient<LoginPage>();
                builder.Services.AddTransient<RatingListPage>();
                builder.Services.AddTransient<RatingDetailPage>();
                builder.Services.AddTransient<RatingFormPage>();
                builder.Services.AddTransient<DatabasesListPage>();
                builder.Services.AddTransient<DatabaseEditPage>();

                
                return builder.Build();
            }
        }
    }
