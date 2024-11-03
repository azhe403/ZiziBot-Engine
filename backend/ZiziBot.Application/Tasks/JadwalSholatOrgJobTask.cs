using Hangfire;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ZiziBot.Application.Tasks;

public class JadwalSholatOrgJobTask(DataFacade dataFacade) : IStartupTask
{
    public bool SkipAwait { get; set; }

    public async Task ExecuteAsync()
    {
        RecurringJob.AddOrUpdate<JadwalSholatOrgSinkService>(
            CronJobKey.JadwalSholatOrg_FetchAll,
            methodCall: service => service.FeedAll(),
            queue: CronJobKey.Queue_ShalatTime,
            cronExpression: TimeUtil.MonthInterval(1)
        );

        var checkCity = await dataFacade.MongoEf.JadwalSholatOrg_City.AsNoTracking()
            .Where(entity => entity.Status == EventStatus.Complete)
            .CountAsync();

        var checkSchedule = await dataFacade.MongoEf.JadwalSholatOrg_Schedule.AsNoTracking()
            .Where(entity => entity.Status == EventStatus.Complete)
            .CountAsync();

        if (checkCity > 0 || checkSchedule > 0)
        {
            Log.Information("JadwalSholat.org data is already exist, skip seeding data at startup.");
            return;
        }

        RecurringJob.TriggerJob(CronJobKey.JadwalSholatOrg_FetchAll);
    }
}