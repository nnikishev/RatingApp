namespace RatingApp.Models
{
public class Dashboard : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Layout { get; set; } // JSON layout
        public bool IsPublic { get; set; } = false;
        public string ChartsConfig { get; set; } // JSON с конфигурацией чартов
    }
}