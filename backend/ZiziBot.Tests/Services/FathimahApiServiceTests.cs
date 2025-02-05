using Xunit;

namespace ZiziBot.Tests.Services;

public class FathimahApiServiceTests(
    FathimahApiService fathimahApiService
)
{
    public static readonly object[][] ShalatCity = [
        [DateTime.Now, 1203],
        // [DateTime.Now.AddMonths(-2).AddDays(10).AddYears(-1), 1203],
        // [new DateTime(2022, 3, 1), 1203]
    ];

    [Fact]
    public async Task GetListCityTest()
    {
        var allCity = await fathimahApiService.GetAllCityAsync();

        allCity.Status.ShouldBe(true);
        allCity.Cities.ShouldNotBeEmpty();
    }

    [Theory, MemberData(nameof(ShalatCity))]
    public async Task GetShalatTimeTest(DateTime dateTime, int cityId)
    {
        var shalatTime = await fathimahApiService.GetShalatTime(dateTime, cityId);

        shalatTime.Status.ShouldBe(true);
        shalatTime.Schedule.ShouldNotBeNull()
                  .ShalatDict.ShouldNotBeNull().ShouldNotBeEmpty();
    }
}