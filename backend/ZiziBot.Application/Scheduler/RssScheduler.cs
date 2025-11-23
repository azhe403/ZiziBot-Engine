using System.ComponentModel;
using Hangfire;
using Microsoft.Extensions.Logging;
using Sentry.Hangfire;
using ZiziBot.Application.UseCases.Rss;

namespace ZiziBot.Application.Scheduler;

public class RssScheduler(
    ILogger<RssScheduler> logger,
    RegisterRssJobAllUseCase registerRssJobAllUseCase,
    FetchRssUseCase fetchRssUseCase
)
{
    [DisplayName("RSS {0}:{1} {2}")]
    [AutomaticRetry(Attempts = 1, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    [SentryMonitorSlug("FetchRssUseCase")]
    [Queue(CronJobKey.Queue_Rss)]
    public async Task RssBroadcast(long chatId, int threadId, string rssUrl)
    {
        await fetchRssUseCase.Handle(chatId: chatId, rssUrl: rssUrl, threadId: threadId);
    }

    [DisplayName("RSS Refresh")]
    [AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    [SentryMonitorSlug("RegisterRssJobAllUseCase")]
    [Queue(CronJobKey.Queue_Data)]
    public async Task RssRefresh()
    {
        await registerRssJobAllUseCase.Handle(new RegisterRssJobAllRequest()
        {
            ResetStatus = true
        });
    }
}