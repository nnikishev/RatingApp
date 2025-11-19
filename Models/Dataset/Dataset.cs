using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace RatingApp.Models
{  
    [ObservableObject]
    public partial class Dataset : BaseEntity
    {
        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private string sqlQuery = string.Empty;

        [ObservableProperty]
        private int databaseId;

        [ObservableProperty]
        private DateTime createdAt = DateTime.Now;

        [ObservableProperty]
        private DateTime updatedAt = DateTime.Now;

        // Навигационное свойство (не сохраняется в БД)
        [Ignore]
        public Database Database { get; set; }
    }
}