using Microsoft.EntityFrameworkCore;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.Application.Handlers.Telegram.Group;

public class SetAfkBotRequest : BotRequestBase
{
    public string? Reason { get; set; }
}

public class SetAfkRequestHandler(
    DataFacade dataFacade,
    ServiceFacade serviceFacade
)
    : IRequestHandler<SetAfkBotRequest, BotResponseBase>
{
    public async Task<BotResponseBase> Handle(SetAfkBotRequest request, CancellationToken cancellationToken)
    {
        serviceFacade.TelegramService.SetupResponse(request);

        var afkEntity = await dataFacade.MongoEf.Afk
            .FirstOrDefaultAsync(entity =>
                    entity.UserId == request.UserId &&
                    entity.Status == EventStatus.Complete,
                cancellationToken: cancellationToken);

        if (afkEntity == null)
        {
            dataFacade.MongoEf.Afk.Add(new AfkEntity() {
                UserId = request.UserId,
                ChatId = request.ChatIdentifier,
                Reason = request.Reason,
                Status = EventStatus.Complete,
                TransactionId = request.TransactionId
            });
        }
        else
        {
            afkEntity.Reason = request.Reason;
            afkEntity.TransactionId = request.TransactionId;
            afkEntity.Status = EventStatus.Complete;
        }

        await dataFacade.MongoEf.SaveChangesAsync(cancellationToken);

        var htmlMessage = HtmlMessage.Empty
            .User(request.User)
            .Text(" sedang AFK");

        if (request.Reason != null)
            htmlMessage.Br()
                .Bold("Alasan: ").Text(request.Reason);

        return await serviceFacade.TelegramService.SendMessageText(htmlMessage.ToString());
    }
}