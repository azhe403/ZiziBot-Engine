using Xunit;
using ZiziBot.Database.Service;

namespace ZiziBot.Tests.Features;

public class CacheTowerTests(CacheService cacheService)
{
    [Fact]
    public async Task WriteToCache()
    {
        var testData1 = "testData1";

        var cacheData1 = await cacheService.GetOrSetAsync(cacheKey: "test", action: () => Task.FromResult(testData1));
        testData1.ShouldBeEquivalentTo(cacheData1);

        var guidData = Guid.NewGuid();
        var cacheGuid = await cacheService.GetOrSetAsync(cacheKey: "test-guid", action: () => Task.FromResult(guidData));
        guidData.ShouldBeEquivalentTo(cacheGuid);
    }
}