using CodeHollow.FeedReader;
using Flurl;
using Octokit;
using ZiziBot.Common.Utils;
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
            if (throwIfError)
                throw;

            Log.Error(e, "Error reading rss: {RssUrl}", rssUrl);

            return new Feed();
        }
    }

    public static async Task<string> DetectRss(this string rssUrl, int attempt = 1, bool throwIfError = false)
    {
        try
        {
            var fixedUrl = rssUrl.TrimEnd("/");

            while (true)
            {
                const int maxAttempt = 3;

                if (attempt > maxAttempt)
                    throw new Exception($"Unable to detect rss Url: {rssUrl}");

                var readRss = await rssUrl.ReadRssAsync();
                if (!readRss.Items.IsEmpty())
                    return fixedUrl;

                if ((rssUrl.IsGithubReleaseUrl() || rssUrl.IsGithubCommitsUrl()) && !fixedUrl.EndsWith(".atom"))
                {
                    fixedUrl += ".atom";
                    var read = await fixedUrl.ReadRssAsync();
                    if (!read.Items.IsEmpty())
                        return fixedUrl;
                }

                if (!rssUrl.EndsWith("/feed"))
                {
                    fixedUrl = rssUrl + "/feed";
                    var read = await fixedUrl.ReadRssAsync();
                    if (!read.Items.IsEmpty())
                        return fixedUrl;
                }

                if (!rssUrl.EndsWith("/rss"))
                {
                    fixedUrl = rssUrl.AppendPathSegment("rss");
                    var read = await fixedUrl.ReadRssAsync();
                    if (!read.Items.IsEmpty())
                        return fixedUrl;
                }

                var urlParse = rssUrl.UrlParse();
                var urlParseScheme = urlParse.Scheme + "://" + urlParse.Host;

                fixedUrl = urlParseScheme;
                attempt += 1;
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Unable detect RSS from: {RssUrl}", rssUrl);

            if (throwIfError)
                throw;

            return rssUrl;
        }
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

        Log.Debug("Collecting GitHub assets latest for URL: {Url}", url);
        var client = new GitHubClient(new ProductHeaderValue("ZiziBot"));

        if (!string.IsNullOrWhiteSpace(token))
            client.Credentials = new Credentials(token);

        var repoGroup = url.Split("/")[3];
        var repoName = url.Split("/")[4];

        Log.Debug("Getting GitHub release assets latest for URL: {Url}", url);
        var release = await client.Repository.Release.GetLatest(repoGroup, repoName);

        Log.Debug("Got Assets for URl: {Url} {Count} item(s)", url, release.Assets.Count);
        return release;
    }

    public static async Task<IReadOnlyList<Release>?> GetGithubAssets(this string url, string? token = null)
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
        var release = await client.Repository.Release.GetAll(repoGroup, repoName);

        Log.Debug("Got Assets for URl: {Url} {Count} item(s)", url, release.Count);
        return release;
    }
}