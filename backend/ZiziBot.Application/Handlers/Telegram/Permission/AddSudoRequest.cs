namespace ZiziBot.Application.Handlers.Telegram.Permission;

public class AddSudoBotRequestModel : BotRequestBase
{
    public long CustomUserId { get; set; }
}

[UsedImplicitly]
public class AddSudoRequestHandler : IRequestHandler<AddSudoBotRequestModel, BotResponseBase>
{
    private readonly TelegramService _telegramService;
    private readonly SudoService _sudoService;

    public AddSudoRequestHandler(TelegramService telegramService, AppSettingsDbContext appSettingsDbContext, SudoService sudoService)
    {
        _telegramService = telegramService;
        _sudoService = sudoService;
    }

    public async Task<BotResponseBase> Handle(AddSudoBotRequestModel request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        await _telegramService.SendMessageText("Adding sudo user...");

        var serviceResult = await _sudoService.SaveSudo(new SudoerEntity()
        {
            UserId = request.CustomUserId == 0 ? request.UserId : request.CustomUserId,
            PromotedBy = request.UserId,
            PromotedFrom = request.ChatIdentifier,
            Status = (int)EventStatus.Complete
        });

        await _telegramService.EditMessageText(serviceResult.Message);

        return _telegramService.Complete();
    }
}