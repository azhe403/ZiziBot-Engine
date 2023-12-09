using FluentAssertions;
using Xunit;

namespace ZiziBot.Tests.Features;

public class JadwalSholatOrgParserTests
{
    [Fact]
    public async Task GetCitiesTest()
    {
        var cities = await JadwalSholatOrgParserUtil.GetCities();

        cities.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(1)]
    public async Task FetchPerCity_FullYear_Test(int cityId)
    {
        var shalatTimes = await JadwalSholatOrgParserUtil.FetchSchedules(cityId);

        shalatTimes.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(1)]
    public async Task Fetch_CustomMonth_Test(int cityId)
    {
        var year = DateTime.UtcNow.Year;

        var shalatTimes = await JadwalSholatOrgParserUtil.FetchSchedules(cityId, year);

        shalatTimes.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(1)]
    public async Task Fetch_CustomMonth_CustomYear_Test(int cityId)
    {
        var year = DateTime.UtcNow.Year;
        var month = DateTime.UtcNow.Month;

        var shalatTimes = await JadwalSholatOrgParserUtil.FetchSchedules(cityId, year, month);

        shalatTimes.Should().NotBeNullOrEmpty();
    }
}