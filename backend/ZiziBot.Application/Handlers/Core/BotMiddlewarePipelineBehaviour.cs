using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Core;

public class BotMiddlewarePipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : RequestBase, IRequest<TResponse>
{
    private readonly ILogger<BotMiddlewarePipelineBehaviour<TRequest, TResponse>> _logger;
    private readonly TelegramService _telegramService;
    private readonly IMediator _mediator;

    public BotMiddlewarePipelineBehaviour(
        ILogger<BotMiddlewarePipelineBehaviour<TRequest, TResponse>> logger,
        TelegramService telegramService,
        IMediator mediator
    )
    {
        _logger = logger;
        _telegramService = telegramService;
        _mediator = mediator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var commonMessage = string.Empty;
        _telegramService.SetupResponse(request);

        var checkAntispam = await _mediator.Send(new AntiSpamRequestModel()
            {
                UserId = request.UserId,
                ChatId = request.ChatIdentifier,
                User = request.User
            },
            cancellationToken);

        if (!checkAntispam.CanContinue)
        {
            await _telegramService.DeleteMessageAsync();
            await _telegramService.SendMessageText(checkAntispam.Message);

            throw new UserGlobalBannedException(request.UserId);
        }

        // todo. execute another middlewares

        return await next();
    }
}