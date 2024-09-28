using MongoFramework.Linq;
using ZiziBot.DataSource.MongoDb.Entities;

namespace ZiziBot.Application.Handlers.Telegram.Ban;

public class AddBanBotRequest : BotRequestBase
{
}

public class AddBanBotHandler(TelegramService telegramService, MongoDbContextBase mongoDbContext) : IRequestHandler<AddBanBotRequest, BotResponseBase>
{
    public async Task<BotResponseBase> Handle(AddBanBotRequest request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        var htmlMessage = HtmlMessage.Empty;
        var userId = request.MessageTexts?.Skip(1).FirstOrDefault().Convert<long>();
        var reason = request.Param.Replace(userId.ToString(), "");

        if (request.ReplyToMessage != null)
        {
            userId = request.ReplyToMessage.From?.Id;
        }

        if (userId == 0)
        {
            return await telegramService.SendMessageText("Spesifikasikan User yang ingin diblokir.");
        }

        var globalBan = await mongoDbContext.GlobalBan
            .Where(entity => entity.UserId == userId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (globalBan == null)
        {
            mongoDbContext.GlobalBan.Add(new GlobalBanEntity() {
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

        await mongoDbContext.SaveChangesAsync(cancellationToken);

        htmlMessage.Bold("Pengguna berhasi diban").Br()
            .Bold("UserID: ").CodeBr(userId.ToString());

        return await telegramService.SendMessageText(htmlMessage.ToString());
    }
}