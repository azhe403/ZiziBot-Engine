using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Core;

public class BotMiddlewarePipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : BotRequestBase, IRequest<TResponse>
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
        request.CleanupTargets = new[]
        {
            CleanupTarget.FromBot
        };

        _telegramService.SetupResponse(request);

        if (request.Source != ResponseSource.Bot)
        {
            _logger.LogDebug("Awatiting next because Request Source is: {Source}", request.Source);
            return await next();
        }

        var result = await _mediator.Send(new BotMiddlewareRunnerRequest()
            {
                UserId = request.UserId,
                ChatId = request.ChatIdentifier,
                User = request.User
            },
            cancellationToken);

        if (result.CanContinue)
        {
            return await next();
        }

        await _telegramService.SendMessageText(text: result.Message, replyMarkup: result.ReplyMarkup);

        if (result.MuteDuration != TimeSpan.Zero)
            await _telegramService.MuteMemberAsync(userId: request.UserId, duration: result.MuteDuration);

        if (result.DeleteMessage)
            await _telegramService.DeleteMessageAsync();

        throw new BotMiddlewareException<TRequest>("User Not Passed");
    }
}