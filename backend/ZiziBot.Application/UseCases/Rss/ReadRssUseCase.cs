using Humanizer;
using Microsoft.Extensions.Logging;
using MoreLinq;
using Octokit;
using ZiziBot.Common.Types;
using ZiziBot.Database.MongoDb;

namespace ZiziBot.Application.UseCases.Rss;

public class ReadRssResponse
{
    public string Link { get; set; }
    public string Title { get; set; }
    public List<ReadRssItem> Items { get; set; }
}

public class ReadRssItem
{
    public string Link { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public DateTime PublishDate { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public List<Release> Assets { get; set; }
}

public class Assets
{
    public string Url { get; set; }
    public string Name { get; set; }
    public int Size { get; set; }
}

public class ReadRssUseCase(
    ILogger<ReadRssUseCase> logger,
    ICacheService cacheService,
    MongoDbContext mongoDbContext,
    AppSettingRepository appSettingRepository,
    FeatureFlagRepository featureFlagRepository
)
{
    public async Task<ReadRssResponse> Handle(string rssUrl)
    {
        logger.LogInformation("Reading RSS: {Url}", rssUrl);

        var isGithubReleaseUrl = rssUrl.IsGithubReleaseUrl();
        var isGithubCommitsUrl = rssUrl.IsGithubCommitsUrl();
        var includeRssContent = await featureFlagRepository.IsEnabled(Flag.RSS_INCLUDE_CONTENT);

        var feed = await cacheService.GetOrSetAsync("rss/" + rssUrl.UrlDecode(), async () => {
            var feed = await rssUrl.ReadRssAsync(throwIfError: true);

            var readRssResponse = new ReadRssResponse() {
                Link = rssUrl,
                Title = feed.Title,
            };

            if (isGithubReleaseUrl)
            {
                var githubReleases = await rssUrl.GetGithubAssets(Env.GithubToken);
                var item = githubReleases?.Take(3).Select(feedItem => {
                    var messageText = HtmlMessage.Empty
                        .Url(feed.Link, feed.Title.Trim()).Br()
                        .Url(feedItem.HtmlUrl, feedItem.Name.Trim()).Br();

                    var releaseAssets = feedItem.Assets;

                    if (releaseAssets?.NotEmpty() ?? false)
                    {
                        messageText.Br()
                            .BoldBr("Assets");

                        releaseAssets.ForEach(asset => {
                            messageText.Url(asset.BrowserDownloadUrl, asset.Name).Br();
                        });
                    }

                    return new ReadRssItem {
                        Link = feedItem.HtmlUrl,
                        Title = feedItem.Name,
                        Author = feedItem.Author.Login,
                        PublishDate = feedItem.PublishedAt?.UtcDateTime ?? DateTime.UtcNow,
                        Content = messageText.ToString(),
                    };
                });

                readRssResponse.Items = item?.ToList() ?? [];
            }
            else
            {
                var readRssItems = feed.Items.Take(3).Select(async feedItem => {
                    var htmlContent = await feedItem.Content.HtmlForTelegram();

                    var messageText = HtmlMessage.Empty
                        .Url(feed.Link, feed.Title.Trim()).Br()
                        .Url(feedItem.Link, feedItem.Title.Trim()).Br();

                    if (!isGithubCommitsUrl && includeRssContent)
                        messageText.Text(htmlContent.Truncate(2000));

                    var truncatedMessageText = messageText.ToString();

                    return new ReadRssItem {
                        Link = feedItem.Link,
                        Title = feedItem.Title,
                        Author = feedItem.Author,
                        PublishDate = feedItem.PublishingDate ?? DateTime.UtcNow,
                        Content = truncatedMessageText,
                        Description = feedItem.Description
                    };
                }).ToList();

                var rssItems = await Task.WhenAll(readRssItems);

                readRssResponse.Items = rssItems.ToList();
            }

            return readRssResponse;
        }, throwIfError: true);

        return feed;
    }
}