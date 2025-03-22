using System.ComponentModel;
using Hangfire;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoreLinq;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.Application.UseCases.Rss;

public class FetchRssUseCase(
    ILogger<FetchRssUseCase> logger,
    DataFacade dataFacade,
    ServiceFacade serviceFacade
)
{
    [DisplayName("{0}:{1} -> {2}")]
    [AutomaticRetry(Attempts = 2, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task<bool> Handle(long chatId, int threadId, string rssUrl)
    {
        logger.LogInformation("Processing RSS Url: {Url}", rssUrl);
        var botSettings = await dataFacade.AppSetting.GetBotMain();

        try
        {
            var feed = await rssUrl.ReadRssAsync(throwIfError: true);

            foreach (var latestArticle in feed.Items.Take(3))
            {
                var latestHistory = await dataFacade.Rss.GetLastRssArticle(chatId, threadId, latestArticle.Link);

                if (latestHistory != null)
                {
                    logger.LogDebug("No new article found in ChatId: {ChatId} for RSS Url: {Url}", chatId, rssUrl);
                    continue;
                }

                var botClient = new TelegramBotClient(botSettings.Token);

                var htmlContent = await latestArticle.Content.HtmlForTelegram();

                var messageText = HtmlMessage.Empty
                    .Url(feed.Link, feed.Title.Trim()).Br()
                    .Url(latestArticle.Link, latestArticle.Title.Trim()).Br();

                if (!rssUrl.IsGithubCommitsUrl() &&
                    await dataFacade.FeatureFlag.GetFlagValue(Flag.RSS_INCLUDE_CONTENT))
                    messageText.Text(htmlContent.Truncate(2000));

                if (rssUrl.IsGithubReleaseUrl())
                {
                    var assets = await rssUrl.GetGithubAssetLatest();

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
                        chatId: chatId,
                        messageThreadId: threadId,
                        text: truncatedMessageText,
                        parseMode: ParseMode.Html,
                        linkPreviewOptions: true
                    );
                }
                catch (Exception exception)
                {
                    if (exception.Message.Contains("thread not found"))
                    {
                        logger.LogWarning(exception, "Trying send RSS without thread to ChatId: {ChatId}", chatId);
                        await botClient.SendMessage(
                            chatId: chatId,
                            text: truncatedMessageText,
                            parseMode: ParseMode.Html,
                            linkPreviewOptions: true
                        );
                    }
                }

                dataFacade.MongoEf.RssHistory.Add(new() {
                    ChatId = chatId,
                    ThreadId = threadId,
                    RssUrl = rssUrl,
                    Title = latestArticle.Title,
                    Url = latestArticle.Link,
                    Author = latestArticle.Author,
                    PublishDate = latestArticle.PublishingDate.GetValueOrDefault(DateTime.Now),
                    Status = EventStatus.Complete
                });
            }
        }
        catch (Exception exception)
        {
            if (exception.IsIgnorable())
            {
                var findRssSetting = await dataFacade.MongoEf.RssSetting
                    .Where(entity => entity.ChatId == chatId)
                    .Where(entity => entity.RssUrl == rssUrl)
                    .Where(entity => entity.Status == EventStatus.Complete)
                    .ToListAsync();

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
                logger.LogError(exception, "Error while sending RSS article to Chat: {ChatId}. Url: {Url}", chatId, rssUrl);
            }
        }

        await dataFacade.MongoEf.SaveChangesAsync();

        return true;
    }
}