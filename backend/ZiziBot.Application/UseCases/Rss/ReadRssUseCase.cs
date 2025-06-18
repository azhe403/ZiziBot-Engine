using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoreLinq;
using Octokit;

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

public class ReadRssUseCase(ILogger<ReadRssUseCase> logger, ServiceFacade serviceFacade, DataFacade dataFacade)
{
    public async Task<ReadRssResponse> Handle(string rssUrl)
    {
        logger.LogInformation("Reading RSS: {Url}", rssUrl);

        var isGithubReleaseUrl = rssUrl.IsGithubReleaseUrl();
        var includeRssContent = await dataFacade.FeatureFlag.GetFlagValue(Flag.RSS_INCLUDE_CONTENT);

        var feed = await serviceFacade.CacheService.GetOrSetAsync("rss/" + rssUrl, async () => {
            var feed = await rssUrl.ReadRssAsync(throwIfError: true);

            var readRssResponse = new ReadRssResponse() {
                Link = rssUrl,
                Title = feed.Title,
            };

            var isGithubCommitsUrl = rssUrl.IsGithubCommitsUrl();

            var readRssItems = feed.Items.Skip(5).Select(async feedItem => {
                var htmlContent = await feedItem.Content.HtmlForTelegram();

                var messageText = HtmlMessage.Empty
                    .Url(feed.Link, feed.Title.Trim()).Br()
                    .Url(feedItem.Link, feedItem.Title.Trim()).Br();

                if (!isGithubCommitsUrl && includeRssContent)
                    messageText.Text(htmlContent.Truncate(2000));

                if (isGithubReleaseUrl)
                {
                    var apiKey = await dataFacade.MongoEf.ApiKey
                        .OrderBy(x => x.LastUsedDate)
                        .Where(x => x.Name == ApiKeyVendor.GitHub)
                        .Where(x => x.Status == EventStatus.Complete)
                        .FirstOrDefaultAsync();

                    var githubApiKey = apiKey?.ApiKey;

                    var assets = await rssUrl.GetGithubAssetLatest(githubApiKey);

                    var releaseAssets = assets?.Assets;

                    if (releaseAssets?.NotEmpty() ?? false)
                    {
                        messageText.Br()
                            .BoldBr("Assets");

                        releaseAssets.ForEach(asset => {
                            messageText.Url(asset.BrowserDownloadUrl, asset.Name).Br();
                        });
                    }
                }

                var truncatedMessageText = messageText.ToString();

                await dataFacade.SaveChangesAsync();

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

            return readRssResponse;
        });

        return feed;
    }
}