using CodeHollow.FeedReader;

namespace ZiziBot.Parsers.WebParser;

public static class RssParserUtil
{
    public static async Task<Feed> ReadRssAsync(this string rssUrl)
    {
        var feed = await FeedReader.ReadAsync(rssUrl, userAgent: Env.COMMON_UA);
        return feed;
    }
}