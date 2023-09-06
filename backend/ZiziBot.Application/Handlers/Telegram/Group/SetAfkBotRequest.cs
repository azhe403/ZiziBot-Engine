using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Group;

public class SetAfkBotRequest : BotRequestBase
{
    public string? Reason { get; set; }
}

public class SetAfkRequestHandler : IRequestHandler<SetAfkBotRequest, BotResponseBase>
{
    private readonly TelegramService _telegramService;
    private readonly MongoDbContextBase _mongoDbContext;

    public SetAfkRequestHandler(TelegramService telegramService, MongoDbContextBase mongoDbContext)
    {
        _telegramService = telegramService;
        _mongoDbContext = mongoDbContext;
    }

    public async Task<BotResponseBase> Handle(SetAfkBotRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        var afkEntity = await _mongoDbContext.Afk
            .FirstOrDefaultAsync(entity =>
                    entity.UserId == request.UserId &&
                    entity.Status == (int)EventStatus.Complete,
                cancellationToken: cancellationToken);

        if (afkEntity == null)
        {
            _mongoDbContext.Afk.Add(new AfkEntity()
            {
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

        await _mongoDbContext.SaveChangesAsync(cancellationToken);

        var htmlMessage = HtmlMessage.Empty
            .User(request.User)
            .Text(" sedang AFK");

        if (request.Reason != null)
            htmlMessage.Br()
                .Bold("Alasan: ").Text(request.Reason);

        return await _telegramService.SendMessageText(htmlMessage.ToString());
    }
}