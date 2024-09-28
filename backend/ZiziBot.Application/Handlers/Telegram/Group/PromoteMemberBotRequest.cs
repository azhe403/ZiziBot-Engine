namespace ZiziBot.Application.Handlers.Telegram.Group;

public class PromoteMemberBotRequest : BotRequestBase
{
    public bool Promote { get; set; }
}

public class PromoteMemberHandler(TelegramService telegramService) : IRequestHandler<PromoteMemberBotRequest, BotResponseBase>
{
    public async Task<BotResponseBase> Handle(PromoteMemberBotRequest request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        if (request.Promote)
        {

            if (await telegramService.CheckAdministration())
                return await telegramService.SendMessageText("Pengguna sudah menjadi admin");

            await telegramService.PromoteMember(request.UserId);
            return await telegramService.SendMessageText("Promote berhasil");
        }
        else
        {
            if (!await telegramService.CheckAdministration())
                return await telegramService.SendMessageText("Pengguna sudah bukan lagi admin");

            await telegramService.DemoteMember(request.UserId);
            return await telegramService.SendMessageText("Demote berhasil");
        }
    }
}