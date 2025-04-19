using Xunit;

namespace ZiziBot.Tests.Features;

public class JadwalSholatOrgParserTests
{
    public readonly static object[][] FetchCustomMonthCustomYear = [
        [1, DateTime.Now.Month, DateTime.Now.Year],
        [1, DateTime.Now.Month, DateTime.Now.AddMonths(-2).AddDays(10).AddYears(-1).Year],
    ];

    [Fact]
    public async Task GetCitiesTest()
    {
        var cities = await JadwalSholatOrgParserUtil.GetCities();

        cities.ShouldNotBeNull();
        cities.Count.ShouldBeGreaterThan(0);
    }

    [Theory]
    [InlineData(1)]
    public async Task FetchPerCity_FullYear_Test(int cityId)
    {
        var shalatTimes = await JadwalSholatOrgParserUtil.FetchSchedules(cityId);

        shalatTimes.ShouldNotBeNull();
        shalatTimes.Count.ShouldBeGreaterThan(0);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(1, 10)]
    public async Task Fetch_CustomMonth_Test(int cityId, int month)
    {
        var shalatTimes = await JadwalSholatOrgParserUtil.FetchSchedules(cityId, month);

        shalatTimes.ShouldNotBeNull();
        shalatTimes.Count.ShouldBeGreaterThan(0);
    }

    [Theory, MemberData(nameof(FetchCustomMonthCustomYear))]
    public async Task Fetch_CustomMonth_CustomYear_Test(int cityId, int month, int year)
    {
        var shalatTimes = await JadwalSholatOrgParserUtil.FetchSchedules(cityId, month, year);

        shalatTimes.ShouldNotBeNull();
        shalatTimes.Count.ShouldBeGreaterThan(0);
    }
}