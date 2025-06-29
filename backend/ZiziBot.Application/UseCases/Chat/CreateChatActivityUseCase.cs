using Hangfire;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Telegram.Bot.Types;
using ZiziBot.Database.MongoDb.Entities;

namespace ZiziBot.Application.UseCases.Chat;

public class CreateChatActivityRequest
{
    public ChatActivityType ActivityType { get; set; }
    public Message SentMessage { get; set; }
    public string TransactionId { get; set; }
}

public class CreateChatActivityUseCase(
    DataFacade dataFacade,
    ServiceFacade serviceFacade
)
{
    [MaximumConcurrentExecutions(3)]
    public async Task<bool> Handle(CreateChatActivityRequest request)
    {
        dataFacade.MongoDb.ChatActivity.Add(new ChatActivityEntity {
            ActivityType = request.ActivityType,
            ActivityTypeName = request.ActivityType.ToString(),
            ChatId = request.SentMessage.Chat.Id,
            UserId = request.SentMessage.From?.Id,
            Status = EventStatus.Complete,
            TransactionId = request.TransactionId,
            MessageId = request.SentMessage.MessageId
        });

        var oldActivity = await dataFacade.MongoDb.ChatActivity
            .Where(x => x.CreatedDate <= DateTime.UtcNow.AddMonths(-2))
            .ToListAsync();

        if (oldActivity.Count != 0)
        {
            Log.Information("Delete Chat Activity Count: {Count} in 2 months", oldActivity.Count);
            dataFacade.MongoDb.ChatActivity.RemoveRange(oldActivity);
        }

        await dataFacade.MongoDb.SaveChangesAsync();

        return true;
    }
}