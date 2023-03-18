using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Group;

public class SetAfkRequest : RequestBase
{
    public string? Reason { get; set; }
}

public class SetAfkRequestHandler : IRequestHandler<SetAfkRequest, ResponseBase>
{
    private readonly TelegramService _telegramService;
    private readonly ChatDbContext _chatDbContext;

    public SetAfkRequestHandler(TelegramService telegramService, ChatDbContext chatDbContext)
    {
        _telegramService = telegramService;
        _chatDbContext = chatDbContext;
    }

    public async Task<ResponseBase> Handle(SetAfkRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        var afkEntity = await _chatDbContext.Afk
            .FirstOrDefaultAsync(entity =>
                    entity.UserId == request.UserId &&
                    entity.Status == (int) EventStatus.Complete,
                cancellationToken: cancellationToken);

        if (afkEntity == null)
        {
            _chatDbContext.Afk.Add(new AfkEntity()
            {
                UserId = request.UserId,
                ChatId = request.ChatIdentifier,
                Reason = request.Reason,
                Status = (int) EventStatus.Complete
            });
        }
        else
        {
            afkEntity.Reason = request.Reason;
            afkEntity.TransactionId = Guid.NewGuid().ToString();
            afkEntity.Status = (int) EventStatus.Complete;
        }

        await _chatDbContext.SaveChangesAsync(cancellationToken);

        var htmlMessage = HtmlMessage.Empty
            .User(request.User)
            .Text(" sedang AFK");

        if (request.Reason != null)
            htmlMessage.Br()
                .Bold("Alasan: ").Text(request.Reason);

        return await _telegramService.SendMessageText(htmlMessage.ToString());
    }
}