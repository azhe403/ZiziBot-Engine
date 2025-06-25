using Microsoft.EntityFrameworkCore;
using Xunit;
using ZiziBot.Application.Facades;
using ZiziBot.Common.Enums;

namespace ZiziBot.Tests.Pipelines;

public class RssTests(MediatorService mediatorService, DataFacade dataFacade)
{
    [Theory]
    [InlineData("https://devblogs.microsoft.com/dotnet/feed/")]
    [InlineData("https://github.com/j-hc/revanced-magisk-module/releases.atom")]
    [InlineData("https://github.com/revanced-apks/build-apps/releases.atom")]
    public async Task FetchTest(string url)
    {
        var historyEntity = await dataFacade.MongoEf.RssHistory
            .Where(entity => entity.RssUrl == url)
            .ToListAsync();

        historyEntity.ForEach(x => {
            x.Status = (int)EventStatus.Deleted;
        });

        await dataFacade.MongoEf.SaveChangesAsync();


        await mediatorService.Send(new FetchRssRequest() {
            ChatId = -1001710313973,
            RssUrl = url
        });
    }
}