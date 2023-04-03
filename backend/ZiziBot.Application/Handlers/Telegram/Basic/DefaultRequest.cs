namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class DefaultRequestModel : RequestBase
{
}

public class DefaultRequestHandler : IRequestHandler<DefaultRequestModel, ResponseBase>
{
    private readonly TelegramService _telegramService;

    public DefaultRequestHandler(TelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public async Task<ResponseBase> Handle(DefaultRequestModel request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        await Task.Delay(1, cancellationToken);

        return _telegramService.Complete();
    }
}