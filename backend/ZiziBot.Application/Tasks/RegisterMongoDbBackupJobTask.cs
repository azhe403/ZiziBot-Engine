using Hangfire;

namespace ZiziBot.Application.Tasks;

public class RegisterMongoDbBackupJobTask : IStartupTask
{
    public bool SkipAwait { get; set; }

    public async Task ExecuteAsync()
    {
        RecurringJob.AddOrUpdate<MediatorService>(
            recurringJobId: "mongodb-backup",
            methodCall: service => service.Send(new MongoDbBackupRequest()),
            cronExpression: TimeUtil.DayInterval(1),
            queue: CronJobKey.Queue_Data
        );

        await Task.Delay(1);
    }
}