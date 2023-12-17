using Hangfire;

namespace ZiziBot.Application.Tasks;

public class JadwalSholatOrgJobTask : IStartupTask
{
    public bool SkipAwait { get; set; }

    public Task ExecuteAsync()
    {
        // RecurringJob.AddOrUpdate<JadwalSholatOrgSinkService>(
        //     recurringJobId: CronJobKey.JadwalSholatOrg_FetchCity,
        //     queue: "shalat-time",
        //     methodCall: service => service.FeedCity(),
        //     cronExpression: TimeUtil.MonthInterval(1));
        //
        // RecurringJob.AddOrUpdate<JadwalSholatOrgSinkService>(
        //     recurringJobId: CronJobKey.JadwalSholatOrg_FetchSchedule,
        //     queue: "shalat-time",
        //     methodCall: service => service.FeedSchedule(),
        //     cronExpression: TimeUtil.MonthInterval(1));

        RecurringJob.AddOrUpdate<JadwalSholatOrgSinkService>(
            recurringJobId: CronJobKey.JadwalSholatOrg_FetchAll,
            queue: "shalat-time",
            methodCall: service => service.FeedAll(),
            cronExpression: TimeUtil.MonthInterval(1));

        // RecurringJob.TriggerJob(CronJobKey.JadwalSholatOrg_FetchCity);
        // RecurringJob.TriggerJob(CronJobKey.JadwalSholatOrg_FetchSchedule);
        RecurringJob.TriggerJob(CronJobKey.JadwalSholatOrg_FetchAll);

        return Task.CompletedTask;
    }
}