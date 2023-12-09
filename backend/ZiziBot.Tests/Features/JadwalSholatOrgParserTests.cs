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
    [InlineData(1, 2)]
    [InlineData(1, 10)]
    public async Task Fetch_CustomMonth_Test(int cityId, int month)
    {
        var shalatTimes = await JadwalSholatOrgParserUtil.FetchSchedules(cityId, month);

        shalatTimes.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(1, 2, 2022)]
    [InlineData(1, 12, 2023)]
    public async Task Fetch_CustomMonth_CustomYear_Test(int cityId, int month, int year)
    {
        var shalatTimes = await JadwalSholatOrgParserUtil.FetchSchedules(cityId, month, year);

        shalatTimes.Should().NotBeNullOrEmpty();
    }
}