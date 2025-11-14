using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RatingApp.Models;
using RatingApp.Services;
using RatingApp.Views;

namespace RatingApp.ViewModels
{
    public partial class RatingListViewModel : BaseViewModel
    {
        private readonly IRatingService _ratingService;
        
        public ObservableCollection<RatingItem> Items { get; } = new();

        public RatingListViewModel(IRatingService ratingService)
        {
            _ratingService = ratingService;
            Title = "Ratings List";
            System.Diagnostics.Debug.WriteLine("VIEWMODEL: RatingListViewModel created");
        }
        
        [RelayCommand]
        private async Task LoadItemsAsync()
        {

            try
            {
                IsBusy = true;
                Items.Clear();

                // Check if database is ready
                var isDbReady = await _ratingService.IsDatabaseReady();
                System.Diagnostics.Debug.WriteLine($"DATABASE_READY: {isDbReady}");

                if (isDbReady)
                {
                    // Load from database
                    var items = await _ratingService.GetItemsAsync();
                    if (items != null && items.Count > 0)
                    {
                        foreach (var item in items)
                        {
                            Items.Add(item);
                        }
                        System.Diagnostics.Debug.WriteLine($"DATABASE_LOAD_SUCCESS: {items.Count} items loaded");
                        return;
                    }
                }

                // Fallback to mock data
                // System.Diagnostics.Debug.WriteLine("DATABASE_FALLBACK: Using mock data");
                // LoadMockData();
                
                if (!isDbReady)
                {
                    await Application.Current.MainPage.DisplayAlert("Info", 
                        "Using sample data (database not available)", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LOAD_ITEMS_ERROR: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Info", 
                    "Using sample data", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }


        [RelayCommand]
        private async Task ItemTappedAsync(RatingItem item)
        {
            System.Diagnostics.Debug.WriteLine("COMMAND: ItemTappedAsync started");
            try
            {
                if (item is null) return;
                
                System.Diagnostics.Debug.WriteLine($"VIEWMODEL: Item selected - {item.Name}");
                await Application.Current.MainPage.DisplayAlert("Selected Item", 
                    $"Name: {item.Name}\nRating: {item.Rating}\nCategory: {item.Category}\nDescription: {item.Description}", 
                    "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"VIEWMODEL_ERROR: ItemTappedAsync failed - {ex}");
            }
        }

        [RelayCommand]
        private async Task DeleteItemAsync(RatingItem item)
        {
            System.Diagnostics.Debug.WriteLine("COMMAND: DeleteItemAsync started");
            try
            {
                bool answer = await Application.Current.MainPage.DisplayAlert(
                    "Confirmation",
                    $"Delete '{item.Name}' from database?",
                    "Yes", "No"); 

                if (answer)
                {
                    System.Diagnostics.Debug.WriteLine("VIEWMODEL: Deleting item from database...");
                    await _ratingService.DeleteItemAsync(item);
                    System.Diagnostics.Debug.WriteLine("VIEWMODEL: Item deleted from database");

                    Items.Remove(item);
                    await Application.Current.MainPage.DisplayAlert("Success", "Item deleted from database", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"VIEWMODEL_ERROR: DeleteItemAsync failed - {ex}");
                await Application.Current.MainPage.DisplayAlert("Error",
                    $"Failed to delete item: {ex.Message}", "OK");
            }
        }

        [RelayCommand]

        private async Task EditItemAsync(RatingItem item)
        {
            if (item is null) return;

            try
            {
                System.Diagnostics.Debug.WriteLine($"EDIT_ITEM: Editing item {item.Name}");

                // Navigate to edit form
                var editPage = new RatingFormPage(_ratingService, item);
                if (Application.Current?.MainPage is NavigationPage navPage)
                {
                    await navPage.PushAsync(editPage);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EDIT_ITEM_ERROR: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to open edit form", "OK");
            }
        }


        [RelayCommand]
        private async Task CreateNewItemAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("CREATE_NEW_ITEM: Opening create form");

                // Navigate to create form
                var createPage = new RatingFormPage(_ratingService);
                if (Application.Current?.MainPage is NavigationPage navPage)
                {
                    await navPage.PushAsync(createPage);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CREATE_NEW_ITEM_ERROR: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to open create form", "OK");
            }
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            System.Diagnostics.Debug.WriteLine("COMMAND: GoBackAsync started");
            try
            {
                if (Application.Current?.MainPage is NavigationPage navPage)
                {
                    await navPage.PopAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"VIEWMODEL_ERROR: GoBackAsync failed - {ex}");
            }
        }
    }
}