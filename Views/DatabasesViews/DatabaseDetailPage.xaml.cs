using RatingApp.ViewModels;
using RatingApp.Models;
using RatingApp.Services;

namespace RatingApp.Views
{
    public partial class DatabaseDetailPage : ContentPage
    {
        public DatabaseDetailPage(DatabaseContext databaseContext, IRatingService ratingService, Database database)
        {
            InitializeComponent();
            BindingContext = new DatabaseDetailViewModel(databaseContext, ratingService, database);
        }
    }
}