using Telegram.Bot.Types;
using ZiziBot.DataSource.MongoDb.Entities;

namespace ZiziBot.Application.Handlers.Telegram.Core;

public class CreateChatActivityRequest : BotRequestBase
{
    public ChatActivityType ActivityType { get; set; }
    public Message SentMessage { get; set; }
}

public class CreateChatActivityHandler(MongoDbContextBase mongoDbContextBase, TelegramService telegramService)
    : IBotRequestHandler<CreateChatActivityRequest>
{
    public async Task<BotResponseBase> Handle(CreateChatActivityRequest request, CancellationToken cancellationToken)
    {
        mongoDbContextBase.ChatActivity.Add(new ChatActivityEntity() {
            ActivityType = request.ActivityType,
            ChatId = request.ChatIdentifier,
            Chat = request.SentMessage.Chat,
            User = request.SentMessage.From,
            Status = (int)EventStatus.Complete,
            TransactionId = request.TransactionId
        });

        await mongoDbContextBase.SaveChangesAsync(cancellationToken);

        return telegramService.Complete();
    }
}