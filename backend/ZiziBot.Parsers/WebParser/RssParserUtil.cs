using CodeHollow.FeedReader;
using Octokit;
using Feed = CodeHollow.FeedReader.Feed;

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

    public static async Task<Release?> GetGithubAssetLatest(this string url)
    {
        if (!url.IsGithubReleaseUrl())
            return default;

        var client = new GitHubClient(new ProductHeaderValue("ZiziBot"));
        var repoGroup = url.Split("/")[3];
        var repoName = url.Split("/")[4];

        var assets = await client.Repository.Release.GetLatest(repoGroup, repoName);

        return assets;
    }
}