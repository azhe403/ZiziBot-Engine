using FluentAssertions;
using Xunit;

namespace ZiziBot.Tests.Features;

public class WebParserTests
{
    [Theory]
    [InlineData("daun")]
    [InlineData("azhe403")]
    public async Task GoogleSearchTest(string search)
    {
        var result = await WebParserUtil.WebSearchText(search);

        result.Should().NotBeNull();
    }
}