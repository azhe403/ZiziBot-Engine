using Hangfire;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Tasks;

public class RegisterRssJobTask(
    ILogger<RegisterRssJobTask> logger,
    AppSettingRepository appSettingRepository,
    MediatorService mediatorService
) : IStartupTask
{
    public bool SkipAwait { get; set; } = false;

    public async Task ExecuteAsync()
    {
        logger.LogInformation("Registering RSS Jobs");
        await mediatorService.Send(new RegisterRssJobAllRequest() {
            ResetStatus = await appSettingRepository.GetFlagValue(Flag.RSS_RESET_AT_STARTUP)
        });

        RecurringJob.AddOrUpdate<MediatorService>(
            recurringJobId: CronJobKey.Rss_Reset,
            methodCall: x => x.Send(new RegisterRssJobAllRequest() {
                ResetStatus = true
            }),
            queue: CronJobKey.Queue_Rss,
            cronExpression: TimeUtil.HourInterval(1)
        );

        logger.LogDebug("Registering RSS Jobs Completed");
    }
}