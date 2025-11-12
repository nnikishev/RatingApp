using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RatingApp.Models;
using RatingApp.Services;

namespace RatingApp.ViewModels
{
    [QueryProperty(nameof(ItemId), "id")]
    public partial class RatingDetailViewModel : BaseViewModel
    {
        private readonly IRatingService _ratingService;
        
        [ObservableProperty]
        private RatingItem _item = new();

        public RatingDetailViewModel(IRatingService ratingService)
        {
            _ratingService = ratingService;
            Title = "Детали элемента";
        }

        [ObservableProperty]
        private int _itemId;

        partial void OnItemIdChanged(int value)
        {
            LoadItem(value);
        }

        private async void LoadItem(int itemId)
        {
            try
            {
                var item = await _ratingService.GetItemAsync(itemId);
                if (item != null)
                {
                    Item = item;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task SaveItemAsync()
        {
            if (Item != null)
            {
                await _ratingService.SaveItemAsync(Item);
                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        private async Task DeleteItemAsync()
        {
            if (Item != null)
            {
                await _ratingService.DeleteItemAsync(Item);
                await Shell.Current.GoToAsync("//ratinglist");
            }
        }
    }
}