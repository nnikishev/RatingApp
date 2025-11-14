using RatingApp.ViewModels;
using RatingApp.Models;

namespace RatingApp.Views
{
    public partial class DatabaseEditPage : ContentPage
    {
        public DatabaseEditPage(DatabaseContext databaseContext, Database database = null)
        {
            InitializeComponent();
            BindingContext = new DatabaseEditViewModel(databaseContext, database);
        }
    }
}