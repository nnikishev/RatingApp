using RatingApp.ViewModels;
using RatingApp.Services;
using RatingApp.Models;

namespace RatingApp.Views
{
    public partial class RatingFormPage : ContentPage
    {
        public RatingFormPage(IRatingService ratingService)
        {
            InitializeComponent();
            var viewModel = new RatingFormViewModel(ratingService);
            viewModel.SetItemForCreate();
            BindingContext = viewModel;
        }

        public RatingFormPage(IRatingService ratingService, RatingItem itemForEdit)
        {
            InitializeComponent();
            var viewModel = new RatingFormViewModel(ratingService);
            viewModel.SetItemForEdit(itemForEdit);
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            // UI will auto-update through data binding
            // No need to manually call OnPropertyChanged
        }
    }
}