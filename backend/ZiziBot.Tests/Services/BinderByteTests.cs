using FluentAssertions;
using Xunit;

namespace ZiziBot.Tests.Services;

public class BinderByteTests
{
    private readonly BinderByteService _binderByteService;

    public BinderByteTests(BinderByteService binderByteService)
    {
        _binderByteService = binderByteService;
    }

    [Theory]
    [InlineData("jne", "8825112045716759")]
    public async Task CheckAwbTest(string courier, string awb)
    {
        var awbInfo = await _binderByteService.CekResiMergedAsync(courier, awb);

        awbInfo.Should().NotBeNull();
    }
}