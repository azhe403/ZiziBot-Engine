using Hangfire;
using MongoFramework.Linq;

namespace ZiziBot.Application.Tasks;

public class RegisterShalatTimeTask : IStartupTask
{
    private readonly MongoDbContextBase _mongoDbContext;

    public bool SkipAwait { get; set; }

    public RegisterShalatTimeTask(MongoDbContextBase mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }

    public async Task ExecuteAsync()
    {
        var cities = await _mongoDbContext.BangHasan_ShalatCity
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .Select(entity => entity.ChatId)
            .Distinct()
            .ToListAsync();

        foreach (var city in cities)
        {
            RecurringJob.AddOrUpdate<MediatorService>(
                recurringJobId: $"ShalatTime:{city}",
                methodCall: mediatorService =>
                    mediatorService.Send(new SendShalatTimeRequest()
                    {
                        ChatId = city,
                    }),
                cronExpression: TimeUtil.MinuteInterval(1),
                queue: CronJobKey.Queue_ShalatTime
            );
        }
    }
}