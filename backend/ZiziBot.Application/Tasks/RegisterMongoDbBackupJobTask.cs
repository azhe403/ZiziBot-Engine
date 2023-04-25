using Hangfire;

namespace ZiziBot.Application.Tasks;

public class RegisterMongoDbBackupJobTask : IStartupTask
{
    public bool SkipAwait { get; set; }

    public async Task ExecuteAsync()
    {
        RecurringJob.AddOrUpdate<MediatorService>("mongodb-backup", service => service.Send(new MainDbBackupRequest()), Cron.Daily);

        await Task.Delay(1);
    }
}