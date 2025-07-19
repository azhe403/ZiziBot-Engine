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
    [InlineData("https://dbeaver.io/feed/", "https://dbeaver.io/feed")]
    [InlineData("https://portapps.io", "https://portapps.io/feed")]
    [InlineData("https://portapps.io/apps/", "https://portapps.io/feed")]
    [InlineData("https://github.com/telegramdesktop/tdesktop/releases", "https://github.com/telegramdesktop/tdesktop/releases.atom")]
    [InlineData("https://github.com/telegramdesktop/tdesktop/commits/dev/", "https://github.com/telegramdesktop/tdesktop/commits/dev.atom")]
    [InlineData("https://apidog.canny.io/api/changelog/feed.rss", "https://apidog.canny.io/api/changelog/feed.rss")]
    [InlineData("https://wolipop.detik.com/love/d-7995975/kirim-emoji-saat-chat-online-bikin-hubungan-asmara-makin-mesra-ini-alasannya", "https://wolipop.detik.com/rss")]
    [InlineData("https://oto.detik.com/roadtrip/7138/kelana-daihatsu-xenia-di-tanah-minang", "https://oto.detik.com/rss")]
    public async Task TryFixRssUrlTest(string url, string expected)
    {
        var fixedRssUrl = await url.DetectRss();
        var readRss = await fixedRssUrl.ReadRssAsync();

        fixedRssUrl.ShouldBe(expected);

        readRss.ShouldNotBeNull();
        readRss.Items.Count.ShouldBeGreaterThan(0);
    }

    [Theory]
    [InlineData("https://www.google.com/xxxxx")]
    public async Task TryFixRssUrl_Negative_Test(string url)
    {
        var exception = await Should.ThrowAsync<Exception>(() => url.DetectRss());

        exception.ShouldNotBeNull();
        exception.Message.ShouldBe("Unable to detect rss");
    }
}