using Hangfire;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoreLinq;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.Application.Handlers.Telegram.Rss;

[Obsolete("Please use from UseCase")]
public class FetchRssRequest : IRequest<bool>
{
    public long ChatId { get; set; }
    public int ThreadId { get; set; }
    public string RssUrl { get; set; }
}

public class FetchRssHandler(
    ILogger<FetchRssHandler> logger,
    DataFacade dataFacade,
    ServiceFacade serviceFacade
) : IRequestHandler<FetchRssRequest, bool>
{
    public async Task<bool> Handle(FetchRssRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing RSS Url: {Url}", request.RssUrl);

        try
        {
            var feed = await request.RssUrl.ReadRssAsync(throwIfError: true);

            var latestArticle = feed.Items?.FirstOrDefault();

            if (latestArticle == null)
            {
                logger.LogInformation("No article found in ChatId: {ChatId} for RSS Url: {Url}", request.ChatId, request.RssUrl);

                return false;
            }

            var latestHistory = await dataFacade.Rss.GetLastRssArticle(request.ChatId, request.ThreadId, latestArticle.Link);

            if (latestHistory != null)
            {
                logger.LogDebug("No new article found in ChatId: {ChatId} for RSS Url: {Url}", request.ChatId, request.RssUrl);

                return false;
            }

            var botSettings = await dataFacade.AppSetting.GetBotMain();

            var botClient = new TelegramBotClient(botSettings.Token);

            var htmlContent = await latestArticle.Content.HtmlForTelegram();

            var messageText = HtmlMessage.Empty
                .Url(feed.Link, feed.Title.Trim()).Br()
                .Url(latestArticle.Link, latestArticle.Title.Trim()).Br();

            if (!request.RssUrl.IsGithubCommitsUrl() &&
                await dataFacade.FeatureFlag.GetFlagValue(Flag.RSS_INCLUDE_CONTENT))
                messageText.Text(htmlContent.Truncate(2000));

            if (request.RssUrl.IsGithubReleaseUrl())
            {
                var assets = await request.RssUrl.GetGithubAssetLatest();

                if (assets?.Assets.NotEmpty() ?? false)
                {
                    messageText.Br()
                        .BoldBr("Assets");

                    assets.Assets.ForEach(asset => {
                        messageText.Url(asset.BrowserDownloadUrl, asset.Name).Br();
                    });
                }
            }

            var truncatedMessageText = messageText.ToString();

            try
            {
                await botClient.SendMessage(
                    chatId: request.ChatId,
                    messageThreadId: request.ThreadId,
                    text: truncatedMessageText,
                    parseMode: ParseMode.Html,
                    linkPreviewOptions: true,
                    cancellationToken: cancellationToken
                );
            }
            catch (Exception exception)
            {
                if (exception.Message.Contains("thread not found"))
                {
                    logger.LogWarning(exception, "Trying send RSS without thread to ChatId: {ChatId}", request.ChatId);
                    await botClient.SendMessage(
                        chatId: request.ChatId,
                        text: truncatedMessageText,
                        parseMode: ParseMode.Html,
                        linkPreviewOptions: true,
                        cancellationToken: cancellationToken
                    );
                }
            }

            dataFacade.MongoEf.RssHistory.Add(new() {
                ChatId = request.ChatId,
                ThreadId = request.ThreadId,
                RssUrl = request.RssUrl,
                Title = latestArticle.Title,
                Url = latestArticle.Link,
                Author = latestArticle.Author,
                PublishDate = latestArticle.PublishingDate.GetValueOrDefault(DateTime.Now),
                Status = EventStatus.Complete
            });
        }
        catch (Exception exception)
        {
            if (exception.IsIgnorable())
            {
                var findRssSetting = await dataFacade.MongoEf.RssSetting
                    .Where(entity => entity.ChatId == request.ChatId)
                    .Where(entity => entity.RssUrl == request.RssUrl)
                    .Where(entity => entity.Status == EventStatus.Complete)
                    .ToListAsync(cancellationToken);

                var exceptionMessage = exception.InnerException?.Message ?? exception.Message;

                findRssSetting.ForEach(rssSetting => {
                    logger.LogWarning("Removing RSS CronJob for ChatId: {ChatId}, Url: {Url}. Reason: {Message}", rssSetting.ChatId, rssSetting.RssUrl, exceptionMessage);

                    rssSetting.Status = EventStatus.InProgress;
                    rssSetting.LastErrorMessage = exceptionMessage;

                    RecurringJob.RemoveIfExists(rssSetting.CronJobId);
                });
            }
            else
            {
                logger.LogError(exception, "Error while sending RSS article to Chat: {ChatId}. Url: {Url}", request.ChatId, request.RssUrl);
            }
        }

        await dataFacade.MongoEf.SaveChangesAsync(cancellationToken);

        return true;
    }
}