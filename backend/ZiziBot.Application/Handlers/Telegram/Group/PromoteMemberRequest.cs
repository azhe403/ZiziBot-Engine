namespace ZiziBot.Application.Handlers.Telegram.Group;

public class PromoteMemberRequest : RequestBase
{
    public bool Promote { get; set; }
}

public class PromoteMemberHandler : IRequestHandler<PromoteMemberRequest, ResponseBase>
{
    private readonly TelegramService _telegramService;

    public PromoteMemberHandler(TelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public async Task<ResponseBase> Handle(PromoteMemberRequest request, CancellationToken cancellationToken)
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