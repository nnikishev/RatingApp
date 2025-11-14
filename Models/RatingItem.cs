using SQLite;

namespace RatingApp.Models
{
    public class RatingItem : BaseEntity
    {
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        public int Rating { get; set; } = 3;
        
        [MaxLength(50)]
        public string Category { get; set; } = "General";
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        
        public string ImagePath { get; set; } = "default_image.png";
    }
}