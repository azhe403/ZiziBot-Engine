namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class DefaultBotRequestModel : BotRequestBase
{
}

public class DefaultRequestHandler : IRequestHandler<DefaultBotRequestModel, BotResponseBase>
{
    private readonly TelegramService _telegramService;

    public DefaultRequestHandler(TelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public async Task<BotResponseBase> Handle(DefaultBotRequestModel request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        await Task.Delay(1, cancellationToken);

        return _telegramService.Complete();
    }
}