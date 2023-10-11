using FluentAssertions;
using Flurl.Http;
using Xunit;

namespace ZiziBot.Tests.Services;

public class UupDumpServiceTests
{
    private readonly UupDumpService _uupDumpService;

    public static readonly object[][] ShalatCity =
    {
        new object[] { DateOnly.FromDateTime(DateTime.Now), 712 },
        new object[] { new DateOnly(2017, 3, 1), 712 },
    };

    public UupDumpServiceTests(UupDumpService uupDumpService)
    {
        _uupDumpService = uupDumpService;
    }

    [SkippableTheory(typeof(FlurlHttpException))]
    [InlineData("")]
    [InlineData("19041")]
    [InlineData("22000")]
    public async Task GetBuildTest(string search)
    {
        var updates = await _uupDumpService.GetUpdatesAsync(search);

        updates.Should().NotBeNull();
    }
}