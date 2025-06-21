using CodeHollow.FeedReader;
using Flurl;
using Octokit;
using Feed = CodeHollow.FeedReader.Feed;

namespace ZiziBot.Parsers.WebParser;

public static class RssParserUtil
{
    public static async Task<Feed> ReadRssAsync(this string rssUrl, bool throwIfError = false)
    {
        try
        {
            var feed = await FeedReader.ReadAsync(rssUrl, userAgent: Env.COMMON_UA);
            return feed;
        }
        catch (Exception e)
        {
            Log.Error(e, "Error reading rss: {RssUrl}", rssUrl);

            if (throwIfError)
                throw;

            return new Feed();
        }
    }

    public static async Task<string> DetectRss(this string rssUrl)
    {
        var fixedUrl = rssUrl.TrimEnd("/");

        var readRss = await rssUrl.ReadRssAsync();
        if (readRss.Items?.Count > 0)
            return rssUrl;

        if ((rssUrl.IsGithubReleaseUrl() || rssUrl.IsGithubCommitsUrl()) && !fixedUrl.EndsWith(".atom"))
        {
            fixedUrl += ".atom";
            var read = await fixedUrl.ReadRssAsync();
            if (read.Items?.Count > 0)
                return fixedUrl;
        }

        if (!rssUrl.EndsWith("/feed"))
        {
            fixedUrl = rssUrl + "/feed";
            var read = await fixedUrl.ReadRssAsync();
            if (read.Items?.Count > 0)
                return fixedUrl;
        }

        if (!rssUrl.EndsWith("/rss"))
        {
            fixedUrl = rssUrl.AppendPathSegment("rss");
            var read = await fixedUrl.ReadRssAsync();
            if (read.Items?.Count > 0)
                return fixedUrl;
        }

        var urlParse = rssUrl.UrlParse();
        var urlParseScheme = (urlParse.Scheme + "://" + urlParse.Host);
        if (!urlParseScheme.IsValidUrl())
            return rssUrl;

        fixedUrl = await urlParseScheme.DetectRss();
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

    public static bool IsGithubUrl(this string url)
    {
        return url.Contains("github.com");
    }

    public static async Task<Release?> GetGithubAssetLatest(this string url, string? token = null)
    {
        if (!url.IsGithubReleaseUrl())
            return null;

        Log.Debug("Collecting GitHub assets for URL: {Url}", url);
        var client = new GitHubClient(new ProductHeaderValue("ZiziBot"));

        if (!string.IsNullOrWhiteSpace(token))
            client.Credentials = new Credentials(token);

        var repoGroup = url.Split("/")[3];
        var repoName = url.Split("/")[4];

        Log.Debug("Getting GitHub release assets for URL: {Url}", url);
        var release = await client.Repository.Release.GetLatest(repoGroup, repoName);

        Log.Debug("Got Assets for URl: {Url} {Count} item(s)", url, release.Assets.Count);
        return release;
    }
}