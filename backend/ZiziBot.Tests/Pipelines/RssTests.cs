using MongoFramework.Linq;
using Xunit;

namespace ZiziBot.Tests.Pipelines;

public class RssTests
{
    private readonly MediatorService _mediatorService;
    private readonly MongoDbContextBase _mongoDbContextBase;

    public RssTests(MediatorService mediatorService, MongoDbContextBase mongoDbContextBase)
    {
        _mediatorService = mediatorService;
        _mongoDbContextBase = mongoDbContextBase;

    }

    [Theory]
    [InlineData("https://devblogs.microsoft.com/dotnet/feed/")]
    [InlineData("https://github.com/j-hc/revanced-magisk-module/releases.atom")]
    [InlineData("https://github.com/revanced-apks/build-apps/releases.atom")]
    public async Task FetchTest(string url)
    {
        var historyEntity = await _mongoDbContextBase.RssHistory
            .Where(entity => entity.RssUrl == url)
            .ToListAsync();

        historyEntity.ForEach(x => {
            x.Status = (int)EventStatus.Deleted;
        });

        await _mongoDbContextBase.SaveChangesAsync();


        await _mediatorService.Send(new FetchRssRequest()
        {
            ChatId = -1001710313973,
            RssUrl = url
        });
    }
}