namespace RatingApp.Models
{
public class Source : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int DatabaseId { get; set; }
        public string Query { get; set; }
        public string TableName { get; set; }
        public string Fields { get; set; } // JSON с полями
        public bool IsActive { get; set; } = true;
    }
}
