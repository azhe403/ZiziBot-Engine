using CodeHollow.FeedReader;

namespace ZiziBot.Parsers.WebParser;

public static class RssParserUtil
{
    public static async Task<Feed> ReadRssAsync(this string rssUrl)
    {
        var feed = await FeedReader.ReadAsync(rssUrl, userAgent: Env.COMMON_UA);
        return feed;
    }

    public static string TryFixRssUrl(this string rssUrl)
    {
        var fixedUrl = rssUrl;

        if (rssUrl.EndsWith("feed"))
            fixedUrl = rssUrl + "/";

        if ((rssUrl.IsGithubReleaseUrl() || rssUrl.IsGithubCommitsUrl()) &&
            !rssUrl.EndsWith(".atom")) fixedUrl = rssUrl + ".atom";

        Log.Debug(
            "Try fix Rss URL: {Url}. After fix: {FixedUrl}",
            rssUrl,
            fixedUrl
        );

        return fixedUrl;
    }

    public static bool IsGithubReleaseUrl(this string url)
    {
        return url.Contains("github.com") && url.Contains("releases");
    }

    public static bool IsGithubCommitsUrl(this string url)
    {
        return url.Contains("github.com") && url.Contains("commits");
    }
}