namespace RatingApp.Models
{
    // Enum для типов баз данных
    public enum DatabaseType
    {
        MySQL,
        PostgreSQL,
        SQLite,
        SQLServer,
        Oracle,
        MongoDB,
        Cassandra,
        Redis,
        ClickHouse,
        BigQuery,
        Snowflake
    }

    // Enum для типов диаграмм
    public enum ChartType
    {
        Bar,
        Line,
        Pie,
        Area,
        Scatter,
        Bubble,
        Radar,
        Heatmap,
        Treemap,
        Sunburst,
        Gauge,
        Table
    }
}