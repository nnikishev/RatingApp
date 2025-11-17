using RatingApp.ViewModels;

namespace RatingApp.Views
{
    public partial class SqlShellPage : ContentPage
    {
        public SqlShellPage(Models.Database database)
        {
            InitializeComponent();
            BindingContext = new SqlShellViewModel(database);
        }
    }
}