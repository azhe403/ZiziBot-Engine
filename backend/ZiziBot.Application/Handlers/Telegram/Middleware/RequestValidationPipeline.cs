using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Telegram.Middleware;

public class RequestValidationPipeline : IPipelineBehavior<BotMiddlewareRunnerRequest, BotMiddlewareResponseBase<AntiSpamDto>>
{
    private readonly ILogger<RequestValidationPipeline> _logger;

    public RequestValidationPipeline(ILogger<RequestValidationPipeline> logger)
    {
        _logger = logger;
    }

    public async Task<BotMiddlewareResponseBase<AntiSpamDto>> Handle(
        BotMiddlewareRunnerRequest request,
        RequestHandlerDelegate<BotMiddlewareResponseBase<AntiSpamDto>> next,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Validating request pipeline for {Request}", request.GetType());

        var response = new BotMiddlewareResponseBase<AntiSpamDto>()
        {
            CanContinue = true
        };

        if (request.UserId != 0)
            await next();

        if (request.ChatId != 0)
            await next();

        return response;
    }
}