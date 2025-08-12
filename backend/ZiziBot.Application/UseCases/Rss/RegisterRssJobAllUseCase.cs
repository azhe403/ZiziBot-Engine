using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.UseCases.Rss;

public class RegisterRssJobAllRequest
{
    public bool ResetStatus { get; set; }
}

public class RegisterRssJobAllUseCase(
    ILogger<RegisterRssJobAllUseCase> logger,
    RegisterRssJobUrlUseCase registerRssJobUrlUseCase,
    DataFacade dataFacade,
    ServiceFacade serviceFacade
)
{
    public async Task<bool> Handle(RegisterRssJobAllRequest request)
    {
        var transactionId = Guid.NewGuid().ToString();

        if (request.ResetStatus)
        {
            var rssSettingsAll = await dataFacade.MongoDb.RssSetting.ToListAsync();

            rssSettingsAll.ForEach(entity =>
            {
                entity.LastErrorMessage = string.Empty;
                entity.Status = EventStatus.Complete;
            });

            await dataFacade.MongoDb.SaveChangesAsync();
        }

        HangfireUtil.RemoveRssJobs();

        var rssSettings = await dataFacade.MongoDb.RssSetting
            .Where(entity => entity.Status == EventStatus.Complete)
            .ToListAsync();

        logger.LogInformation("Registering RSS Jobs. Count: {Count}", rssSettings.Count);

        await rssSettings.ParallelForEachAsync(async rss =>
        {
            try
            {
                var jobId = $"{CronJobKey.Rss_Prefix}:{rss.Id}";
                var rssUrl = await rss.RssUrl.DetectRss(throwIfError: true);

                await registerRssJobUrlUseCase.Handle(new RegisterRssJobUrlRequest()
                {
                    ChatId = rss.ChatId,
                    ThreadId = rss.ThreadId,
                    Url = rssUrl,
                    JobId = jobId
                });

                rss.RssUrl = rssUrl;
                rss.CronJobId = jobId;
                rss.TransactionId = transactionId;
            }
            catch (Exception e)
            {
                rss.Status = EventStatus.Inactive;
                rss.LastErrorMessage = e.Message;

                logger.LogError(e, "Error registering RSS Job. RssUrl: {RssUrl}, ChatId: {ChatId}, ThreadId: {ThreadId}", rss.RssUrl, rss.ChatId, rss.ThreadId);
            }
        });

        await dataFacade.MongoDb.SaveChangesAsync();

        return true;
    }
}