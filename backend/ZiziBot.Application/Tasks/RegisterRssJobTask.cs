using Hangfire;
using Microsoft.Extensions.Logging;
using ZiziBot.Application.UseCases.GitHub;
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
        if (EnvUtil.IsEnabled(Flag.RSS_BROADCASTER))
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

            RecurringJob.AddOrUpdate<UpdateStatisticUseCase>(
                recurringJobId: "update-github-usage",
                methodCall: x => x.Handle(),
                queue: CronJobKey.Queue_Rss,
                cronExpression: TimeUtil.MinuteInterval(3)
            );

            logger.LogDebug("Registering RSS Jobs Completed");
        }
        else
        {
            RecurringJob.RemoveIfExists(CronJobKey.Rss_Reset);
        }
    }
}