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
}