using RatingApp.ViewModels;
using RatingApp.Services;

namespace RatingApp.Views
{
    public partial class RatingListPage : ContentPage
    {
        public RatingListPage(IRatingService ratingService)
        {
            InitializeComponent();
            BindingContext = new RatingListViewModel(ratingService);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            // Автоматически загружаем данные при появлении страницы
            if (BindingContext is RatingListViewModel viewModel)
            {
                viewModel.LoadItemsCommand?.Execute(null);
            }
        }
    }
}