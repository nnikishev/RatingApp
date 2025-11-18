using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RatingApp.Models;
using RatingApp.Services;
using RatingApp.Views;

namespace RatingApp.ViewModels
{
    public partial class DatasetsListViewModel : ObservableObject
    {
        private readonly IDatasetService _datasetService;
        private readonly IRatingService _ratingService;

        [ObservableProperty]
        private List<Dataset> datasets = new();

        [ObservableProperty]
        private bool isLoading;

        public DatasetsListViewModel(IDatasetService datasetService, IRatingService ratingService)
        {
            _datasetService = datasetService;
            _ratingService = ratingService;
            
            LoadDatasetsAsync().SafeFireAndForget();
        }

        [RelayCommand]
        private async Task LoadDatasetsAsync()
        {
            try
            {
                IsLoading = true;
                
                var datasets = await _datasetService.GetDatasetsAsync();
                
                // Загружаем связанные базы данных
                foreach (var dataset in datasets)
                {
                    if (dataset.DatabaseId > 0)
                    {
                        dataset.Database = await _ratingService.GetDatabaseAsync(dataset.DatabaseId);
                    }
                }
                
                Datasets = datasets;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", 
                    $"Не удалось загрузить наборы данных: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task AddDatasetAsync()
        {
            var editPage = new DatasetEditPage(_datasetService, _ratingService);
            await Application.Current.MainPage.Navigation.PushAsync(editPage);
        }

        [RelayCommand]
        private async Task EditDatasetAsync(Dataset dataset)
        {
            if (dataset == null) return;
            
            var editPage = new DatasetEditPage(_datasetService, _ratingService, dataset);
            await Application.Current.MainPage.Navigation.PushAsync(editPage);
        }

        [RelayCommand]
        private async Task DeleteDatasetAsync(Dataset dataset)
        {
            if (dataset == null) return;

            var confirm = await Application.Current.MainPage.DisplayAlert(
                "Удаление набора данных",
                $"Вы уверены, что хотите удалить набор данных \"{dataset.Name}\"?",
                "Удалить",
                "Отмена");

            if (confirm)
            {
                try
                {
                    await _datasetService.DeleteDatasetAsync(dataset);
                    await LoadDatasetsAsync();
                    
                    await Application.Current.MainPage.DisplayAlert(
                        "Успех", 
                        "Набор данных удален", 
                        "OK");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Ошибка", 
                        $"Не удалось удалить набор данных: {ex.Message}", 
                        "OK");
                }
            }
        }

        [RelayCommand]
        private async Task ViewDatasetAsync(Dataset dataset)
        {
            if (dataset == null) return;
            
            // Здесь можно открыть страницу просмотра данных набора
            await Application.Current.MainPage.DisplayAlert(
                "Просмотр набора", 
                $"Будет открыт набор: {dataset.Name}", 
                "OK");
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}