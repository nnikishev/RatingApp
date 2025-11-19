using RatingApp.ViewModels;
using RatingApp.Models;
using RatingApp.Services;

namespace RatingApp.Views
{
    public partial class DatabasesListPage : ContentPage
    {
        public DatabasesListPage(DatabaseContext databaseContext, IRatingService ratingService, IDatasetService datasetService)
        {
            InitializeComponent();
            BindingContext = new DatabasesListViewModel(databaseContext, ratingService, datasetService);
        }
    }
}