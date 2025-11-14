namespace RatingApp.Models
{
    public class Chart : BaseEntity
    {
        public string Title { get; set; }
        public string ChartType { get; set; } // Bar, Line, Pie, etc.
        public int SourceId { get; set; }
        public string Configuration { get; set; } // JSON config
        public string Dimensions { get; set; } // JSON с измерениями
        public string Measures { get; set; } // JSON с мерами
    }
}