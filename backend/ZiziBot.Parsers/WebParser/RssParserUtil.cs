using CodeHollow.FeedReader;
using Flurl;
using Octokit;
using Feed = CodeHollow.FeedReader.Feed;

namespace ZiziBot.Parsers.WebParser;

public static class RssParserUtil
{
    public static async Task<Feed> ReadRssAsync(this string rssUrl)
    {
        try
        {
            var feed = await FeedReader.ReadAsync(rssUrl, userAgent: Env.COMMON_UA);
            return feed;
        }
        catch (Exception e)
        {
            Log.Error(e, "Error reading rss: {RssUrl}", rssUrl);
            return new();
        }
    }

    public static async Task<string> DetectRss(this string rssUrl)
    {
        var fixedUrl = rssUrl.TrimEnd("/");

        if ((rssUrl.IsGithubReleaseUrl() || rssUrl.IsGithubCommitsUrl()) && !fixedUrl.EndsWith(".atom"))
        {
            fixedUrl += ".atom";
            var read = await fixedUrl.ReadRssAsync();
            if (read.Items?.Count > 0)
                return fixedUrl;
        }
        else
        {
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
        }


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