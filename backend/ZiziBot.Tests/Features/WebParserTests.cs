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

        result.ShouldNotBeNull();
    }

    [Theory]
    [InlineData("https://www.bloombergtechnoz.com/", "https://www.bloombergtechnoz.com/rss")]
    [InlineData("https://dbeaver.io", "https://dbeaver.io/feed")]
    [InlineData("https://portapps.io", "https://portapps.io/feed")]
    [InlineData("https://portapps.io/apps/", "https://portapps.io/feed")]
    [InlineData("https://github.com/telegramdesktop/tdesktop/releases", "https://github.com/telegramdesktop/tdesktop/releases.atom")]
    [InlineData("https://github.com/telegramdesktop/tdesktop/commits/dev/", "https://github.com/telegramdesktop/tdesktop/commits/dev.atom")]
    public async Task TryFixRssUrlTest(string url, string expected)
    {
        var fixedRssUrl = await url.DetectRss();
        var readRss = await fixedRssUrl.ReadRssAsync();

        fixedRssUrl.ShouldBe(expected);

        readRss.ShouldNotBeNull();
        readRss.Items.Count.ShouldBeGreaterThan(0);
    }
}