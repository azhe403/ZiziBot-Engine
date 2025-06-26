using System.ComponentModel.DataAnnotations;

namespace ZiziBot.Database.Sqlite.Entities;

public class SqliteCacheEntity
{
    [Key]
    public required string CacheKey { get; set; }

    public DateTime ExpiryDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? Value { get; set; }
}