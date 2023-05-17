namespace ZiziBot.Application.Handlers.Telegram.Group;

public class PromoteMemberBotRequest : BotRequestBase
{
    public bool Promote { get; set; }
}

public class PromoteMemberHandler : IRequestHandler<PromoteMemberBotRequest, BotResponseBase>
{
    private readonly TelegramService _telegramService;

    public PromoteMemberHandler(TelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public async Task<BotResponseBase> Handle(PromoteMemberBotRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        if (request.Promote)
        {

            if (await _telegramService.CheckAdministration())
                return await _telegramService.SendMessageText("Pengguna sudah menjadi admin");

            await _telegramService.PromoteMember(request.UserId);
            return await _telegramService.SendMessageText("Promote berhasil");
        }
        else
        {
            if (!await _telegramService.CheckAdministration())
                return await _telegramService.SendMessageText("Pengguna sudah bukan lagi admin");

            await _telegramService.DemoteMember(request.UserId);
            return await _telegramService.SendMessageText("Demote berhasil");
        }
    }
}