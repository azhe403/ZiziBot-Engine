using System.ComponentModel;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sentry.Hangfire;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using ZiziBot.Database.MongoDb;
using ZiziBot.Database.MongoDb.Entities;

namespace ZiziBot.Application.UseCases.Rss;

public class FetchRssUseCase(
    ILogger<FetchRssUseCase> logger,
    MongoDbContext mongoDbContext,
    AppSettingRepository appSettingRepository,
    FeatureFlagRepository featureFlagRepository,
    RssRepository rssRepository,
    ReadRssUseCase readRssUseCase
)
{
    [DisplayName("{0}:{1} -> {2}")]
    [AutomaticRetry(Attempts = 2, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    [SentryMonitorSlug("FetchRssUseCase")]
    [Queue(CronJobKey.Queue_Rss)]
    public async Task<bool> Handle(long chatId, int threadId, string rssUrl)
    {
        if (!await featureFlagRepository.IsEnabled(Flag.RSS_BROADCASTER))
            return true;

        logger.LogInformation("Processing RSS Url: {Url}", rssUrl);
        var botSettings = await appSettingRepository.GetBotMain();

        try
        {
            var botClient = new TelegramBotClient(botSettings.Token);
            var feed = await readRssUseCase.Handle(rssUrl);

            if (feed.Items.Count == 0)
            {
                logger.LogWarning("No RSS article found for ChatId: {ChatId}. Url: {Url}", chatId, rssUrl);
                return true;
            }

            foreach (var latestArticle in feed.Items.Take(3))
            {
                var latestHistory = await rssRepository.GetLastRssArticle(chatId, threadId, latestArticle.Link);

                if (latestHistory != null)
                {
                    logger.LogDebug("Article for ChatId: {ChatId} is already sent: {Url}", chatId, latestHistory.Url);
                    continue;
                }

                try
                {
                    await botClient.SendMessage(
                        chatId: chatId,
                        messageThreadId: threadId,
                        text: latestArticle.Content,
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
                            text: latestArticle.Content,
                            parseMode: ParseMode.Html,
                            linkPreviewOptions: true
                        );
                    }
                }

                mongoDbContext.RssHistory.Add(new RssHistoryEntity {
                    ChatId = chatId,
                    ThreadId = threadId,
                    RssUrl = rssUrl,
                    Title = latestArticle.Title,
                    Url = latestArticle.Link,
                    Author = latestArticle.Author,
                    PublishDate = latestArticle.PublishDate,
                    Status = EventStatus.Complete
                });
            }
        }
        catch (Exception exception)
        {
            if (exception.IsIgnorable())
            {
                var findRssSetting = await mongoDbContext.RssSetting
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

        await mongoDbContext.SaveChangesAsync();

        return true;
    }
}