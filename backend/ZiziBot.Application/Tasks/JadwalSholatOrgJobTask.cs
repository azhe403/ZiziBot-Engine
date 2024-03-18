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
        RecurringJob.AddOrUpdate<JadwalSholatOrgSinkService>(
            recurringJobId: CronJobKey.JadwalSholatOrg_FetchAll,
            methodCall: service => service.FeedAll(),
            queue: CronJobKey.Queue_ShalatTime,
            cronExpression: TimeUtil.MonthInterval(1)
        );

        var checkCity = await _mongoDbContextBase.JadwalSholatOrg_City.AsNoTracking()
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .CountAsync();

        var checkSchedule = await _mongoDbContextBase.JadwalSholatOrg_Schedule.AsNoTracking()
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .CountAsync();

        if (checkCity > 0 || checkSchedule > 0)
        {
            Log.Information("JadwalSholat.org data is already exist, skip seeding data at startup.");
            return;
        }

        RecurringJob.TriggerJob(CronJobKey.JadwalSholatOrg_FetchAll);
    }
}