using Xunit;

namespace ZiziBot.Tests.Features;

public class CacheTowerTests
{
    private readonly CacheService _cacheService;

    public CacheTowerTests(CacheService cacheService)
    {
        _cacheService = cacheService;
    }

    [Fact]
    public async Task WriteToCache()
    {
        var testData1 = "testData1";

        var cacheData1 = await _cacheService.GetOrSetAsync(cacheKey: "test", action: () => Task.FromResult(testData1));
        Assert.NotNull(cacheData1);

        var guidData = Guid.NewGuid();
        var cacheGuid = await _cacheService.GetOrSetAsync(cacheKey: "test-guid", action: () => Task.FromResult(guidData));
        Assert.NotNull(cacheGuid);
    }
}