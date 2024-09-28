namespace ZiziBot.Application.Behaviours;

public class CheckChatAdminBehavior(TelegramService telegramService) : IPipelineBehavior<BotRequestBase, BotResponseBase>
{
    public async Task<BotResponseBase> Handle(BotRequestBase request, RequestHandlerDelegate<BotResponseBase> next, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        var checkAdministration = await telegramService.CheckAdministration();

        if (checkAdministration)
        {
            return await next();
        }

        throw new UnauthorizedAccessException($"UserId: {request.UserId} is not a Administrator in ChatId: {request.ChatIdentifier} ");
    }
}