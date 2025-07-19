namespace ZiziBot.Common.Types;

public class Cache<T>
{
    public string CacheKey { get; set; }
    public Func<Task<T>> Action { get; set; }
    public bool DisableCache { get; set; }
    public bool EvictBefore { get; set; }
    public bool EvictAfter { get; set; }
    public string? ExpireAfter { get; set; }
    public string? StaleAfter { get; set; }
    public bool ThrowIfError { get; set; }
}