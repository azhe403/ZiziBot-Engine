using Hangfire;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Services;

public class JobService(ILogger<JobService> logger)
{
    public async Task<bool> Register(long chatId, int threadId, string rssUrl, string jobId)
    {
        logger.LogDebug("Registering RSS Job. ChatId: {ChatId}, ThreadId: {ThreadId} RssUrl: {RssUrl}", chatId, threadId, rssUrl);

        RecurringJob.RemoveIfExists(jobId);
        RecurringJob.AddOrUpdate<MediatorService>(
            recurringJobId: jobId,
            methodCall: mediatorService => mediatorService.Send(new FetchRssRequest() {
                ChatId = chatId,
                ThreadId = threadId,
                RssUrl = rssUrl
            }),
            queue: CronJobKey.Queue_Rss,
            cronExpression: TimeUtil.MinuteInterval(3));

        await Task.Delay(1);

        return true;
    }
}