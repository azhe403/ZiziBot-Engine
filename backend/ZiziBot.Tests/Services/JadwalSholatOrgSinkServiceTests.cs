using Microsoft.EntityFrameworkCore;
using MoreLinq;
using SharpX.Extensions;
using Xunit;
using ZiziBot.Application.Facades;

namespace ZiziBot.Tests.Services;

public class JadwalSholatOrgSinkServiceTests(JadwalSholatOrgSinkService jadwalSholatOrgSinkService, DataFacade dataFacade)
{
    [Fact(Skip = "Deprecated")]
    public async Task FeedCityTest()
    {
        await dataFacade.MongoEf.JadwalSholatOrg_City.Where(entity => true).ExecuteDeleteAsync();
        await dataFacade.MongoEf.SaveChangesAsync();

        await jadwalSholatOrgSinkService.FeedCity();
    }

    [Fact(Skip = "Deprecated")]
    public async Task FeedScheduleTest()
    {
        var cities = await dataFacade.MongoEf.JadwalSholatOrg_City.ToListAsync();

        var randomCities = cities.Shuffle().Take(3);

        await randomCities.ForEachAsync(async entity => await jadwalSholatOrgSinkService.FeedSchedule(entity.CityId));
    }

    [Theory(Skip = "Deprecated")]
    [InlineData(124)]
    public async Task FeedSchedule_ByCity_Test(int cityId)
    {
        var feedSchedule = await jadwalSholatOrgSinkService.FeedSchedule(cityId);

        feedSchedule.ShouldBeGreaterThan(0, "Expected FeedSchedule to insert at least one schedule for the given cityId");
    }
}