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
            var rssSettingsAll = await dataFacade.MongoEf.RssSetting.ToListAsync();

            rssSettingsAll.ForEach(entity => {
                entity.LastErrorMessage = string.Empty;
                entity.Status = EventStatus.Complete;
            });

            await dataFacade.MongoEf.SaveChangesAsync();
        }

        HangfireUtil.RemoveRssJobs();

        var rssSettings = await dataFacade.MongoEf.RssSetting
            .Where(entity => entity.Status == EventStatus.Complete)
            .ToListAsync();

        logger.LogInformation("Registering RSS Jobs. Count: {Count}", rssSettings.Count);

        foreach (var rssSettingEntity in rssSettings)
        {
            var jobId = await StringUtil.GetNanoIdAsync(prefix: "RssJob:", size: 7);

            await registerRssJobUrlUseCase.Handle(new RegisterRssJobUrlRequest() {
                ChatId = rssSettingEntity.ChatId,
                ThreadId = rssSettingEntity.ThreadId,
                Url = rssSettingEntity.RssUrl,
                JobId = jobId
            });

            rssSettingEntity.CronJobId = jobId;
            rssSettingEntity.TransactionId = transactionId;
        }

        await dataFacade.MongoEf.SaveChangesAsync();

        return true;
    }
}