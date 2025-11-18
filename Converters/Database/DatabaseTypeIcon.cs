using System.Globalization;
using RatingApp.Models;

namespace RatingApp.Converters

{
    public class TypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DatabaseType dbType)
            {
                return dbType switch
                {
                    DatabaseType.PostgreSQL => "postgre.png",
                    DatabaseType.MySQL => "database.png", 
                    DatabaseType.SQLServer => "database.png",
                    DatabaseType.SQLite => "database.png",
                    DatabaseType.Oracle => "database.png",
                    _ => "database.png"
                };
            }
            return "database.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}