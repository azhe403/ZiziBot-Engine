using FluentAssertions;
using Xunit;

namespace ZiziBot.Tests.Services;

public class FathimahApiServiceTests
{
    private readonly FathimahApiService _fathimahApiService;

    public static readonly object[][] ShalatCity =
    {
        new object[] { DateTime.Now, 712 },
        new object[] { new DateTime(2017, 3, 1), 712 },
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
        allCity.Cities.Should().NotBeEmpty();
    }

    [Theory, MemberData(nameof(ShalatCity))]
    public async Task GetShalatTimeTest(DateTime dateTime, int cityId)
    {
        var shalatTime = await _fathimahApiService.GetShalatTime(dateTime, cityId);

        shalatTime.Status.Should().Be("ok");
    }
}