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

            rssSettingsAll.ForEach(entity => {
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

        foreach (var rssSettingEntity in rssSettings)
        {
            var jobId = $"{CronJobKey.Rss_Prefix}:{rssSettingEntity.Id}";
            var rssUrl = await rssSettingEntity.RssUrl.DetectRss(throwIfError: false);

            await registerRssJobUrlUseCase.Handle(new RegisterRssJobUrlRequest() {
                ChatId = rssSettingEntity.ChatId,
                ThreadId = rssSettingEntity.ThreadId,
                Url = rssUrl,
                JobId = jobId
            });

            rssSettingEntity.RssUrl = rssUrl;
            rssSettingEntity.CronJobId = jobId;
            rssSettingEntity.TransactionId = transactionId;
        }

        await dataFacade.MongoDb.SaveChangesAsync();

        return true;
    }
}