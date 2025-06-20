using System.ComponentModel;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.Application.UseCases.Rss;

public class FetchRssUseCase(
    ILogger<FetchRssUseCase> logger,
    DataFacade dataFacade,
    ServiceFacade serviceFacade,
    ReadRssUseCase readRssUseCase
)
{
    [DisplayName("{0}:{1} -> {2}")]
    [MaximumConcurrentExecutions(10)]
    [AutomaticRetry(Attempts = 2, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task<bool> Handle(long chatId, int threadId, string rssUrl)
    {
        logger.LogInformation("Processing RSS Url: {Url}", rssUrl);
        var botSettings = await dataFacade.AppSetting.GetBotMain();

        try
        {
            var feed = await readRssUseCase.Handle(rssUrl);

            foreach (var latestArticle in feed.Items.Take(3))
            {
                var latestHistory = await dataFacade.Rss.GetLastRssArticle(chatId, threadId, latestArticle.Link);

                if (latestHistory != null)
                {
                    logger.LogDebug("Article for ChatId: {ChatId} is already sent: {Url}", chatId, latestHistory.Url);
                    continue;
                }

                var botClient = new TelegramBotClient(botSettings.Token);

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

                dataFacade.MongoEf.RssHistory.Add(new RssHistoryEntity {
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