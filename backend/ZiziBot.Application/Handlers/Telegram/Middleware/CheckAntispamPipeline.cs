using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Telegram.Middleware;

public class CheckAntispamPipeline : IPipelineBehavior<BotMiddlewareRunnerRequest, BotMiddlewareResponseBase<AntiSpamDto>>
{
    private readonly ILogger<CheckAntispamPipeline> _logger;
    private readonly AntiSpamService _antiSpamService;

    public CheckAntispamPipeline(ILogger<CheckAntispamPipeline> logger, AntiSpamService antiSpamService)
    {
        _logger = logger;
        _antiSpamService = antiSpamService;
    }

    public async Task<BotMiddlewareResponseBase<AntiSpamDto>> Handle(
        BotMiddlewareRunnerRequest request,
        RequestHandlerDelegate<BotMiddlewareResponseBase<AntiSpamDto>> next,
        CancellationToken cancellationToken
    )
    {
        _logger.LogDebug("Checking antispam for UserId: {UserId} in ChatId: {ChatId}", request.UserId, request.ChatId);

        var response = new BotMiddlewareResponseBase<AntiSpamDto>();
        var antiSpamDto = await _antiSpamService.CheckSpamAsync(request.ChatId, request.UserId);

        response.CanContinue = !antiSpamDto.IsBanAny;

        if (!antiSpamDto.IsBanAny)
            return await next();

        var htmlMessage = HtmlMessage.Empty
            .User(request.UserId, request.User.GetFullName())
            .Text(" is banned from Global Ban");

        response.Message = htmlMessage.ToString();
        response.Result = antiSpamDto;

        return response;
    }
}