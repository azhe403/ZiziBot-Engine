﻿using FluentAssertions;
using Xunit;

namespace ZiziBot.Tests.Features;

public class UrlUtilTests
{
    [Theory]
    [InlineData("https://github.com/azhe403/ZiziBot-Engine")]
    public void GetUrlSegmentTest(string url)
    {
        // Arrange
        var parse0 = url.UrlSegment(0);
        var parse1 = url.UrlSegment(1);

        // Assert
        parse0.Should().Be("azhe403");
        parse1.Should().Be("ZiziBot-Engine");
    }
}