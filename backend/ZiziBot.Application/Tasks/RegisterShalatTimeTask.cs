using Hangfire;
using MongoFramework.Linq;

namespace ZiziBot.Application.Tasks;

public class RegisterShalatTimeTask : IStartupTask
{
    private readonly ChatDbContext _chatDbContext;

    public bool SkipAwait { get; set; }

    public RegisterShalatTimeTask(ChatDbContext chatDbContext)
    {
        _chatDbContext = chatDbContext;
    }

    public async Task ExecuteAsync()
    {
        var cities = await _chatDbContext.City
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
                    }), cronExpression: TimeUtil.DayInterval(1));
        }
    }
}