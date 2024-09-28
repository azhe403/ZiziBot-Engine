namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class DefaultBotRequestModel : BotRequestBase
{
}

public class DefaultRequestHandler(TelegramService telegramService) : IRequestHandler<DefaultBotRequestModel, BotResponseBase>
{
    public async Task<BotResponseBase> Handle(DefaultBotRequestModel request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        await Task.Delay(1, cancellationToken);

        return telegramService.Complete();
    }
}