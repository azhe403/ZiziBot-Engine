using Xunit;

namespace ZiziBot.Tests.Features;

public class CacheTowerTests(CacheService cacheService)
{
    [Fact]
    public async Task WriteToCache()
    {
        var testData1 = "testData1";

        var cacheData1 = await cacheService.GetOrSetAsync(cacheKey: "test", action: () => Task.FromResult(testData1));
        Assert.Equal(testData1, cacheData1);

        var guidData = Guid.NewGuid();
        var cacheGuid = await cacheService.GetOrSetAsync(cacheKey: "test-guid", action: () => Task.FromResult(guidData));
        Assert.Equal(guidData, cacheGuid);
    }
}