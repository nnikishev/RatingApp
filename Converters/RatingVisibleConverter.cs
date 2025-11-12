using System.Globalization;

namespace RatingApp.Converters
{
    public class RatingVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int rating && parameter is string paramString && int.TryParse(paramString, out int starNumber))
            {
                return rating >= starNumber;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}