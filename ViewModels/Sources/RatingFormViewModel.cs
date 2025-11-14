using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RatingApp.Models;
using RatingApp.Services;

namespace RatingApp.ViewModels
{
    public partial class RatingFormViewModel : BaseViewModel
    {
        private readonly IRatingService _ratingService;
        
        [ObservableProperty]
        private RatingItem _item;

        [ObservableProperty]
        private bool _isEditing;

        public List<string> Categories { get; } = new List<string>
        {
            "Electronics",
            "Services", 
            "Food",
            "General",
            "Entertainment",
            "Health",
            "Education"
        };

        public RatingFormViewModel(IRatingService ratingService)
        {
            _ratingService = ratingService;
            _item = new RatingItem();
        }

        public void SetItemForEdit(RatingItem item)
        {
            Item = new RatingItem
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Rating = item.Rating,
                Category = item.Category,
                CreatedDate = item.CreatedDate,
                UpdatedDate = item.UpdatedDate,
                ImagePath = item.ImagePath
            };
            IsEditing = true;
            Title = "Изменить набор";
        }

        public void SetItemForCreate()
        {
            Item = new RatingItem();
            IsEditing = false;
            Title = "Создать новый набор";
        }

        [RelayCommand]
        private void SetRating(string ratingString)
        {
            if (int.TryParse(ratingString, out int rating))
            {
                Item.Rating = rating;
                // UI will auto-update through ObservableProperty
            }
        }

        [RelayCommand]
        private async Task SaveItemAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Item.Name))
                {
                    await Application.Current.MainPage.DisplayAlert("Validation", "Please enter a name", "OK");
                    return;
                }

                if (Item.Rating < 1 || Item.Rating > 5)
                {
                    await Application.Current.MainPage.DisplayAlert("Validation", "Please select a rating between 1 and 5", "OK");
                    return;
                }

                IsBusy = true;

                Item.UpdatedDate = DateTime.Now;

                if (IsEditing)
                {
                    await _ratingService.SaveItemAsync(Item);
                    await Application.Current.MainPage.DisplayAlert("Success", "Item updated successfully", "OK");
                }
                else
                {
                    await _ratingService.SaveItemAsync(Item);
                    await Application.Current.MainPage.DisplayAlert("Success", "Item created successfully", "OK");
                }

                // Navigate back
                if (Application.Current?.MainPage is NavigationPage navPage)
                {
                    await navPage.PopAsync();
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to save item: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            if (Application.Current?.MainPage is NavigationPage navPage)
            {
                await navPage.PopAsync();
            }
        }
    }
}