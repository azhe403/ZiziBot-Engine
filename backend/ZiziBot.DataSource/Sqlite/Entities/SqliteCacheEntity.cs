using System.ComponentModel.DataAnnotations;

namespace ZiziBot.DataSource.Sqlite.Entities;

public class SqliteCacheEntity
{
    [Key]
    public required string CacheKey { get; set; }
    public string? Value { get; set; }
    public DateTime Expiry { get; set; }
}