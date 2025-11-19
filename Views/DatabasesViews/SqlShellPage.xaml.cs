using RatingApp.ViewModels;
using RatingApp.Services;

namespace RatingApp.Views
{
    public partial class SqlShellPage : ContentPage
    {
        public SqlShellPage(Models.Database database, IDatasetService datasetService)
        {
            InitializeComponent();
            BindingContext = new SqlShellViewModel(database, datasetService);
        }
    }
}