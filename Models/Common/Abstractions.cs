using SQLite;

namespace RatingApp.Models
{
    public interface IEntity
    {
        int Id { get; set; }
        DateTime CreatedAt { get; set; }
    }

    public abstract class BaseEntity : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}