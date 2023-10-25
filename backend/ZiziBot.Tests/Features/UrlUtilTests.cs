using FluentAssertions;
using Xunit;

namespace ZiziBot.Tests.Features;

public class UrlUtilTests
{
    [Theory]
    [InlineData("https://github.com/WinTenDev/ZiziBot-Engine")]
    public void GetUrlSegmentTest(string url)
    {
        // Arrange
        var parse0 = url.UrlSegment(0);
        var parse1 = url.UrlSegment(1);

        // Assert
        parse0.Should().Be("WinTenDev");
        parse1.Should().Be("ZiziBot-Engine");
    }
}