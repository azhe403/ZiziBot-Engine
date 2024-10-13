using System.ComponentModel.DataAnnotations;

namespace ZiziBot.DataSource.Sqlite.Entities;

public class SqliteCacheEntity
{
    [Key]
    public required string CacheKey { get; set; }

    public DateTime ExpiryDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? Value { get; set; }
}