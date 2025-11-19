using RatingApp.ViewModels;

namespace RatingApp.Views
{
    public partial class DatasetsListPage : ContentPage
    {
        public DatasetsListPage(DatasetsListViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            if (BindingContext is DatasetsListViewModel viewModel)
            {
                viewModel.LoadDatasetsCommand.Execute(null);
            }
        }
    }
}