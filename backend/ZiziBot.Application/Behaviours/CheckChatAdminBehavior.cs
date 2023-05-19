namespace ZiziBot.Application.Behaviours;

public class CheckChatAdminBehavior : IPipelineBehavior<BotRequestBase, BotResponseBase>
{
    private readonly TelegramService _telegramService;

    public CheckChatAdminBehavior(TelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public async Task<BotResponseBase> Handle(BotRequestBase request, RequestHandlerDelegate<BotResponseBase> next, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        var checkAdministration = await _telegramService.CheckAdministration();

        if (checkAdministration)
        {
            return await next();
        }

        throw new UnauthorizedAccessException($"UserId: {request.UserId} is not a Administrator in ChatId: {request.ChatIdentifier} ");
    }
}