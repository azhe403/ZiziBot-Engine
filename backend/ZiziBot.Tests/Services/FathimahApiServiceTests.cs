using FluentAssertions;
using Xunit;

namespace ZiziBot.Tests.Services;

public class FathimahApiServiceTests
{
    private readonly FathimahApiService _fathimahApiService;

    public static readonly object[][] ShalatCity =
    [
        [DateTime.Now, 1203],
        [DateTime.Now.AddMonths(-2).AddDays(10).AddYears(-1), 1203],
        [new DateTime(2022, 3, 1), 1203]
    ];

    public FathimahApiServiceTests(FathimahApiService fathimahApiService)
    {
        _fathimahApiService = fathimahApiService;
    }

    [Fact]
    public async Task GetListCityTest()
    {
        var allCity = await _fathimahApiService.GetAllCityAsync();

        allCity.Status.Should().Be(true);
        allCity.Cities.Should().NotBeEmpty();
    }

    [Theory, MemberData(nameof(ShalatCity))]
    public async Task GetShalatTimeTest(DateTime dateTime, int cityId)
    {
        var shalatTime = await _fathimahApiService.GetShalatTime(dateTime, cityId);

        shalatTime.Status.Should().Be(true);
    }
}