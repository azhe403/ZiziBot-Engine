using Hangfire;
using Microsoft.EntityFrameworkCore;
using ZiziBot.Application.UseCases.Chat;

namespace ZiziBot.Application.Tasks;

public class RegisterShalatTimeTask(DataFacade dataFacade) : IStartupTask
{
    public async Task ExecuteAsync()
    {
        if (EnvUtil.IsEnabled(Flag.HANGFIRE))
        {
            var cities = await dataFacade.MongoEf.BangHasan_ShalatCity.AsNoTracking()
                .Where(entity => entity.Status == EventStatus.Complete)
                .Select(entity => entity.ChatId)
                .Distinct()
                .ToListAsync();

            foreach (var chatId in cities)
            {
                RecurringJob.AddOrUpdate<SendShalatTimeUseCase>(
                    recurringJobId: $"ShalatTime:{chatId}",
                    methodCall: x => x.Handle(chatId),
                    cronExpression: TimeUtil.MinuteInterval(1),
                    queue: CronJobKey.Queue_ShalatTime
                );
            }
        }
    }
}