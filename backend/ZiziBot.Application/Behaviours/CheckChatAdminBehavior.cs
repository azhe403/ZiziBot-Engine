namespace ZiziBot.Application.Behaviours;

public class CheckChatAdminBehavior : IPipelineBehavior<RequestBase, ResponseBase>
{
    private readonly TelegramService _telegramService;

    public CheckChatAdminBehavior(TelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public async Task<ResponseBase> Handle(RequestBase request, RequestHandlerDelegate<ResponseBase> next, CancellationToken cancellationToken)
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