using FluentAssertions;
using Xunit;

namespace ZiziBot.Tests.Services;

public class TonjooTests
{
    private readonly TonjooService _tonjooService;

    public TonjooTests(TonjooService tonjooService)
    {
        _tonjooService = tonjooService;
    }

    [Theory]
    [InlineData("jne", "8825112045716759")]
    public async Task GetAwbRawTest(string courier, string awb)
    {
        var checkAwb = await _tonjooService.GetAwbInfoRaw(courier, awb);

        checkAwb.Data.Found.Should().BeTrue();
    }

    [Theory]
    [InlineData("jne", "8825112045716759")]
    public async Task GetAwbMergedTest(string courier, string awb)
    {
        var checkAwb = await _tonjooService.GetAwbInfoMerged(courier, awb);

        checkAwb.Should().NotBeNullOrEmpty();
    }
}