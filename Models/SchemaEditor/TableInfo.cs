namespace RatingApp.Models
{
    public class TableInfo
    {
        public string Name { get; set; } = string.Empty;
        public long RowCount { get; set; }
        public string Schema { get; set; } = "public";
    }
}