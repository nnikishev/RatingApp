using SQLite;

namespace RatingApp.Models
{
    public class Database : BaseEntity
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public DatabaseType Type { get; set; }
        public bool IsActive { get; set; } = true;
        public string Description { get; set; }

        [Ignore]
        public int SourcesCount { get; set; }

        [Ignore]
        public string ConnectionString 
        { 
            get
            {
                return Type switch
                {
                    DatabaseType.MySQL => $"Server={Host};Port={Port};Database={Name};Uid={User};Pwd={Password};",
                    DatabaseType.PostgreSQL => $"Host={Host};Port={Port};Database={Name};Username={User};Password={Password};",
                    DatabaseType.SQLServer => $"Server={Host},{Port};Database={Name};User Id={User};Password={Password};",
                    DatabaseType.Oracle => $"Data Source={Host}:{Port}/{Name};User Id={User};Password={Password};",
                    _ => string.Empty
                };
            }
        }
    }
}