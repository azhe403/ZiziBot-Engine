using MediatR.Extensions.AttributedBehaviors;
using Microsoft.Extensions.Logging;
using Nut.MediatR;
using Telegram.Bot.Types;

namespace ZiziBot.Application.Handlers.Telegram.Middleware;

[WithBehaviors(typeof(FluentValidationBehavior<,>))]
[MediatRBehavior(typeof(RequestValidationPipeline))]
// [MediatRBehavior(typeof(CheckUsernamePipeline))]
[MediatRBehavior(typeof(CheckAntispamPipeline))]
public class BotMiddlewareRunnerRequest : BotMiddlewareRequestBase<AntiSpamDto>
{
    public User? User { get; set; }
}

public class BotMiddlewareRunnerHandler : IRequestHandler<BotMiddlewareRunnerRequest, BotMiddlewareResponseBase<AntiSpamDto>>
{
    private readonly ILogger<BotMiddlewareRunnerHandler> _logger;

    public BotMiddlewareRunnerHandler(ILogger<BotMiddlewareRunnerHandler> logger)
    {
        _logger = logger;
    }

    public async Task<BotMiddlewareResponseBase<AntiSpamDto>> Handle(BotMiddlewareRunnerRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Checking antispam for UserId: {UserId} in ChatId: {ChatId}", request.UserId, request.ChatId);

        await Task.Delay(1, cancellationToken);

        var response = new BotMiddlewareResponseBase<AntiSpamDto>
        {
            CanContinue = true
        };

        return response;
    }
}