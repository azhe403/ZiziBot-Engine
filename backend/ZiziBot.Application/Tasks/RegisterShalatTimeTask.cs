using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace ZiziBot.Application.Tasks;

public class RegisterShalatTimeTask(DataFacade dataFacade) : IStartupTask
{
    public bool SkipAwait { get; set; }

    public async Task ExecuteAsync()
    {
        var cities = await dataFacade.MongoEf.BangHasan_ShalatCity
            .Where(entity => entity.Status == EventStatus.Complete)
            .Select(entity => entity.ChatId)
            .Distinct()
            .ToListAsync();

        foreach (var city in cities)
        {
            RecurringJob.AddOrUpdate<MediatorService>(
                $"ShalatTime:{city}",
                methodCall: mediatorService =>
                    mediatorService.Send(new SendShalatTimeRequest() {
                        ChatId = city
                    }),
                cronExpression: TimeUtil.MinuteInterval(1),
                queue: CronJobKey.Queue_ShalatTime
            );
        }
    }
}