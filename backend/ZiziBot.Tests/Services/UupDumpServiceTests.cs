using Xunit;

namespace ZiziBot.Tests.Services;

public class UupDumpServiceTests(UupDumpService uupDumpService)
{
    public static readonly object[][] ShalatCity = {
        new object[] { DateOnly.FromDateTime(DateTime.Now), 712 },
        new object[] { new DateOnly(2017, 3, 1), 712 },
    };

    [InlineData("")]
    [InlineData("19041")]
    [InlineData("22000")]
    public async Task GetBuildTest(string search)
    {
        var updates = await uupDumpService.GetUpdatesAsync(search);

        updates.ShouldNotBeNull();
    }
}