using FluentAssertions;
using Xunit;

namespace ZiziBot.Tests.Services;

public class SubdlServiceTests(SubdlService subdlService)
{
    [Fact]
    public async Task FetchPopularTest()
    {
        // arrange
        var result = await subdlService.FetchPopular();

        // assert
        result.Results.Should().NotBeNullOrEmpty();
    }
}