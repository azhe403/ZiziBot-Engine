using System.ComponentModel;

namespace ZiziBot.Contracts.Configs;

[DisplayName("Cache")]
public class CacheConfig
{
    public bool UseRedis { get; set; }
    public bool UseFirebase { get; set; }
    public bool UseJsonFile { get; set; }
    public bool UseMongoDb { get; set; }
    public bool UseSqlite { get; set; }
    public string? RedisConnection { get; set; }
    public string? FirebaseProjectUrl { get; set; }
    public string? FirebaseServiceAccountJson { get; set; }
}