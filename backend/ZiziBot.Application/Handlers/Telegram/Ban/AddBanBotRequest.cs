using MongoFramework.Linq;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.Application.Handlers.Telegram.Ban;

public class AddBanBotRequest : BotRequestBase
{
}

public class AddBanBotHandler : IRequestHandler<AddBanBotRequest, BotResponseBase>
{
    private readonly TelegramService _telegramService;
    private readonly MongoDbContextBase _mongoDbContext;

    public AddBanBotHandler(TelegramService telegramService, MongoDbContextBase mongoDbContext)
    {
        _telegramService = telegramService;
        _mongoDbContext = mongoDbContext;
    }

    public async Task<BotResponseBase> Handle(AddBanBotRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        var htmlMessage = HtmlMessage.Empty;
        var userId = request.MessageTexts?.Skip(1).FirstOrDefault().Convert<long>();
        var reason = request.Param.Replace(userId.ToString(), "");

        if (userId == 0)
        {
            return await _telegramService.SendMessageText("Spesifikasikan User yang ingin diblokir.");
        }

        var globalBan = await _mongoDbContext.GlobalBan
            .Where(entity => entity.UserId == userId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (globalBan == null)
        {
            _mongoDbContext.GlobalBan.Add(new GlobalBanEntity()
            {
                UserId = userId.GetValueOrDefault(),
                ChatId = request.ChatIdentifier,
                Status = (int)EventStatus.Complete,
                Reason = reason
            });
        }
        else
        {
            globalBan.UserId = userId.GetValueOrDefault();
            globalBan.Reason = reason;
        }

        await _mongoDbContext.SaveChangesAsync(cancellationToken);

        return await _telegramService.SendMessageText(htmlMessage.ToString());
    }
}