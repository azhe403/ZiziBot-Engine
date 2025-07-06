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
    MediatorService mediatorService,
    JobService jobService
) : IStartupTask
{
    public async Task ExecuteAsync()
    {
        if (EnvUtil.IsEnabled(Flag.HANGFIRE))
        {
            if (EnvUtil.IsEnabled(Flag.RSS_BROADCASTER))
            {
                logger.LogInformation("Registering RSS Jobs");
                await registerRssJobAllUseCase.Handle(new RegisterRssJobAllRequest() {
                    ResetStatus = await featureFlagRepository.IsEnabled(Flag.RSS_RESET_AT_STARTUP)
                });

                RecurringJob.AddOrUpdate<RegisterRssJobAllUseCase>(
                    recurringJobId: CronJobKey.Rss_Reset,
                    methodCall: x => x.Handle(new RegisterRssJobAllRequest() {
                        ResetStatus = true
                    }),
                    cronExpression: TimeUtil.HourInterval(1)
                );

                RecurringJob.AddOrUpdate<UpdateStatisticUseCase>(
                    recurringJobId: CronJobKey.GitHub_Update_Token,
                    methodCall: x => x.Handle(),
                    cronExpression: TimeUtil.MinuteInterval(3)
                );

                logger.LogDebug("Registering RSS Jobs Completed");
            }
            else
            {
                RecurringJob.RemoveIfExists(CronJobKey.Rss_Reset);
                HangfireUtil.RemoveRssJobs();
            }
        }
    }
}