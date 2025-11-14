using CommunityToolkit.Mvvm.ComponentModel;
using RatingApp.Services;

namespace RatingApp.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _title = string.Empty;

        private readonly IRatingService _ratingService;
    }
}