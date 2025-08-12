using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using ZiziBot.Database.MongoDb;
using ZiziBot.Database.MongoDb.Entities;

namespace ZiziBot.Application.UseCases.Rss;

public sealed class FetchRssUseCase(
    ILogger<FetchRssUseCase> logger,
    MongoDbContext mongoDbContext,
    BotRepository botRepository,
    FeatureFlagRepository featureFlagRepository,
    RssRepository rssRepository,
    ReadRssUseCase readRssUseCase
)
{
    public async Task<bool> Handle(long chatId, int? threadId, string rssUrl)
    {
        if (!await featureFlagRepository.IsEnabled(Flag.RSS_BROADCASTER))
            return true;

        logger.LogInformation("Processing RSS Url: {Url}", rssUrl);
        var botSettings = await botRepository.GetBotMain();

        try
        {
            var botClient = new TelegramBotClient(botSettings.Token);
            var feed = await readRssUseCase.Handle(rssUrl);

            if (feed.Items.IsEmpty())
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
                    Status = EventStatus.Complete,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                });
            }
        }
        catch (Exception exception)
        {
            if (exception.IsIgnorable())
            {
                logger.LogWarning("Error while sending RSS for ChatId: {ChatId}, Url: {Url}. Reason: {Message}", chatId, rssUrl, exception.Message);
                var findRssSetting = await mongoDbContext.RssSetting
                    .Where(entity => entity.ChatId == chatId)
                    .Where(entity => entity.RssUrl == rssUrl)
                    .Where(entity => entity.Status == EventStatus.Complete)
                    .ToListAsync();

                var exceptionMessage = exception.InnerException?.Message ?? exception.Message;

                findRssSetting.ForEach(rss => {
                    rss.Status = EventStatus.InProgress;
                    rss.LastErrorMessage = exceptionMessage;

                    HangfireUtil.RemoveRecurringJob(rss.CronJobId);
                    logger.LogWarning("Removed RSS CronJob for ChatId: {ChatId}, Url: {Url}. Reason: {Message}", rss.ChatId, rss.RssUrl, exceptionMessage);
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