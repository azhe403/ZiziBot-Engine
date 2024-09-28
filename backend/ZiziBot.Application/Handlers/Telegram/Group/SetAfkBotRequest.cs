using MongoFramework.Linq;
using ZiziBot.DataSource.MongoDb.Entities;

namespace ZiziBot.Application.Handlers.Telegram.Group;

public class SetAfkBotRequest : BotRequestBase
{
    public string? Reason { get; set; }
}

public class SetAfkRequestHandler(TelegramService telegramService, MongoDbContextBase mongoDbContext) : IRequestHandler<SetAfkBotRequest, BotResponseBase>
{
    public async Task<BotResponseBase> Handle(SetAfkBotRequest request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        var afkEntity = await mongoDbContext.Afk
            .FirstOrDefaultAsync(entity =>
                    entity.UserId == request.UserId &&
                    entity.Status == (int)EventStatus.Complete,
                cancellationToken: cancellationToken);

        if (afkEntity == null)
        {
            mongoDbContext.Afk.Add(new AfkEntity() {
                UserId = request.UserId,
                ChatId = request.ChatIdentifier,
                Reason = request.Reason,
                Status = (int)EventStatus.Complete
            });
        }
        else
        {
            afkEntity.Reason = request.Reason;
            afkEntity.TransactionId = Guid.NewGuid().ToString();
            afkEntity.Status = (int)EventStatus.Complete;
        }

        await mongoDbContext.SaveChangesAsync(cancellationToken);

        var htmlMessage = HtmlMessage.Empty
            .User(request.User)
            .Text(" sedang AFK");

        if (request.Reason != null)
            htmlMessage.Br()
                .Bold("Alasan: ").Text(request.Reason);

        return await telegramService.SendMessageText(htmlMessage.ToString());
    }
}