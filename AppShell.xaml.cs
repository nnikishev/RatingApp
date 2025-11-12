using RatingApp.Views;

namespace RatingApp
{
	public partial class AppShell : Shell
	{
		public AppShell()
		{
			InitializeComponent();

			// Регистрируем маршруты
			Routing.RegisterRoute("ratinglist/details", typeof(RatingDetailPage));
		}
	}
}