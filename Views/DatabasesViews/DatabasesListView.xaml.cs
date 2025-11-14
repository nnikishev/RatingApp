using RatingApp.ViewModels.DatabasesViewModels;
using RatingApp.Services;

namespace RatingApp.Views.DatabasesViews
{
    public partial class DatabasesListPage : ContentPage
    {
        public DatabasesListPage(IAuthService authService)
        {
            InitializeComponent();
            BindingContext = new DatabasesListViewModel(authService);
        }
    }
}