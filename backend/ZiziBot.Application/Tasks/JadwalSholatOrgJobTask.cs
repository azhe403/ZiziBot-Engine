using Hangfire;
using MongoFramework.Linq;
using Serilog;

namespace ZiziBot.Application.Tasks;

public class JadwalSholatOrgJobTask : IStartupTask
{
    private readonly MongoDbContextBase _mongoDbContextBase;
    public bool SkipAwait { get; set; }

    public JadwalSholatOrgJobTask(MongoDbContextBase mongoDbContextBase)
    {
        _mongoDbContextBase = mongoDbContextBase;
    }

    public async Task ExecuteAsync()
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

        var checkCity = await _mongoDbContextBase.JadwalSholatOrg_City
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .CountAsync();

        var checkSchedule = await _mongoDbContextBase.JadwalSholatOrg_Schedule
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .CountAsync();

        if (checkCity > 0 || checkSchedule > 0)
        {
            Log.Information("JadwalSholat.org data is already exist, skip seeding data at startup.");
            return;
        }

        // RecurringJob.TriggerJob(CronJobKey.JadwalSholatOrg_FetchCity);
        // RecurringJob.TriggerJob(CronJobKey.JadwalSholatOrg_FetchSchedule);
        RecurringJob.TriggerJob(CronJobKey.JadwalSholatOrg_FetchAll);
    }
}