using System.Globalization;

namespace RatingApp.Converters
{
    public class RatingToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int currentRating && parameter is string paramString && int.TryParse(paramString, out int buttonRating))
            {
                // Return main color if current rating >= button rating, otherwise light gray
                return currentRating >= buttonRating ? "#FF3E3EB" : "#E9ECEF";
            }
            return "#E9ECEF"; // Default light gray
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}