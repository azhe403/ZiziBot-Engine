using Hangfire;
using Microsoft.Extensions.Logging;
using ZiziBot.Application.UseCases.Rss;

namespace ZiziBot.Application.Tasks;

public class RegisterRssJobTask(
    ILogger<RegisterRssJobTask> logger,
    RegisterRssJobAllUseCase registerRssJobAllUseCase,
    AppSettingRepository appSettingRepository,
    FeatureFlagRepository featureFlagRepository,
    MediatorService mediatorService
) : IStartupTask
{
    public bool SkipAwait { get; set; } = false;

    public async Task ExecuteAsync()
    {
        logger.LogInformation("Registering RSS Jobs");

        await registerRssJobAllUseCase.Handle(new RegisterRssJobAllRequest() {
            ResetStatus = await featureFlagRepository.GetFlagValue(Flag.RSS_RESET_AT_STARTUP)
        });

        RecurringJob.AddOrUpdate<RegisterRssJobAllUseCase>(
            recurringJobId: CronJobKey.Rss_Reset,
            methodCall: x => x.Handle(new RegisterRssJobAllRequest() {
                ResetStatus = true
            }),
            queue: CronJobKey.Queue_Rss,
            cronExpression: TimeUtil.HourInterval(1)
        );

        logger.LogDebug("Registering RSS Jobs Completed");
    }
}