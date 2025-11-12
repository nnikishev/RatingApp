using RatingApp.ViewModels;

namespace RatingApp.Views
{
    public partial class RatingDetailPage : ContentPage
    {
        public RatingDetailPage(RatingDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}