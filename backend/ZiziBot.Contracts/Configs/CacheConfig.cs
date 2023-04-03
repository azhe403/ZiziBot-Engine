namespace ZiziBot.Contracts.Configs;

public class CacheConfig
{
    public bool UseRedis { get; set; }
    public bool UseFirebase { get; set; }
    public bool UseJsonFile { get; set; }
    public string? RedisConnection { get; set; }
    public string? FirebaseProjectUrl { get; set; }
    public string? FirebaseServiceAccountJson { get; set; }
}