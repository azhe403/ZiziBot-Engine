using FluentAssertions;
using Xunit;

namespace ZiziBot.Tests.Services;

public class FathimahApiServiceTests
{
    private readonly FathimahApiService _fathimahApiService;

    public static readonly object[][] ShalatCity =
    {
        new object[] { DateOnly.FromDateTime(DateTime.Now), 712},
        new object[] { new DateOnly(2017,3,1), 712},
    };

    public FathimahApiServiceTests(FathimahApiService fathimahApiService)
    {
        _fathimahApiService = fathimahApiService;
    }

    [Fact]
    public async Task GetListCityTest()
    {
        var allCity = await _fathimahApiService.GetAllCityAsync();

        allCity.Status.Should().Be("ok");
        allCity.Kota.Should().NotBeEmpty();
    }

    [Theory, MemberData(nameof(ShalatCity))]
    public async Task GetShalatTimeTest(DateOnly dateTime, int cityId)
    {
        var shalatTime = await _fathimahApiService.GetShalatTime(dateTime, cityId);

        shalatTime.Status.Should().Be("ok");
    }
}