using CommunityToolkit.Mvvm.ComponentModel;
using RatingApp.Services;

namespace RatingApp.ViewModels.DatabasesViewModels
{
    public partial class DatabasesListViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        public DatabasesListViewModel(IAuthService authService)
        {
            _authService = authService;
        }
        
    }
}