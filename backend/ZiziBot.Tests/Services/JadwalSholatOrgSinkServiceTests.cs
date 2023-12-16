using MongoFramework.Linq;
using MoreLinq;
using SharpX.Extensions;
using Xunit;

namespace ZiziBot.Tests.Services;

public class JadwalSholatOrgSinkServiceTests
{
    private readonly MongoDbContextBase _mongoDbContextBase;
    private readonly JadwalSholatOrgSinkService _jadwalSholatOrgSinkService;

    public JadwalSholatOrgSinkServiceTests(JadwalSholatOrgSinkService jadwalSholatOrgSinkService, MongoDbContextBase mongoDbContextBase)
    {
        _mongoDbContextBase = mongoDbContextBase;
        _jadwalSholatOrgSinkService = jadwalSholatOrgSinkService;
    }

    [Fact]
    public async Task FeedCityTest()
    {
        _mongoDbContextBase.JadwalSholatOrg_City.RemoveRange(entity => true);
        await _mongoDbContextBase.SaveChangesAsync();

        await _jadwalSholatOrgSinkService.FeedCity();
    }

    [Fact]
    public async Task FeedScheduleTest()
    {
        var cities = await _mongoDbContextBase.JadwalSholatOrg_City.ToListAsync();

        var randomCities = cities.Shuffle().Take(3);

        await randomCities.ForEachAsync(async entity => await _jadwalSholatOrgSinkService.FeedSchedule(entity.CityId));
    }

    [Theory]
    [InlineData(124)]
    public async Task FeedSchedule_ByCity_Test(int cityId)
    {
        await _jadwalSholatOrgSinkService.FeedSchedule(cityId);
    }
}