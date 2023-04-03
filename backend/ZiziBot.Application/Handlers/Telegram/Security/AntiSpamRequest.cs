using FluentValidation;
using Microsoft.Extensions.Logging;
using Nut.MediatR;
using Telegram.Bot.Types;

namespace ZiziBot.Application.Handlers.Telegram.Security;

[WithBehaviors(typeof(FluentValidationBehavior<,>))]
public class AntiSpamRequestModel : BotMiddlewareRequestBase<AntiSpamDto>
{
    public User? User { get; set; }
}

public class AntiSpamRequestValidation : AbstractValidator<AntiSpamRequestModel>
{
    public AntiSpamRequestValidation()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.ChatId).GreaterThan(0);
    }
}

public class AntiSpamRequest : IRequestHandler<AntiSpamRequestModel, BotMiddlewareResponseBase<AntiSpamDto>>
{
    private readonly ILogger<AntiSpamRequest> _logger;
    private readonly AntiSpamService _antiSpamService;

    public AntiSpamRequest(ILogger<AntiSpamRequest> logger, AntiSpamService antiSpamService)
    {
        _logger = logger;
        _antiSpamService = antiSpamService;
    }

    public async Task<BotMiddlewareResponseBase<AntiSpamDto>> Handle(AntiSpamRequestModel request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Checking antispam for UserId: {UserId} in ChatId: {ChatId}", request.UserId, request.ChatId);

        var response = new BotMiddlewareResponseBase<AntiSpamDto>();
        var combotAntispamApiDto = await _antiSpamService.CheckSpamAsync(request.ChatId, request.UserId);

        response.CanContinue = !combotAntispamApiDto.IsBanAny;

        if (!combotAntispamApiDto.IsBanAny)
            return response;

        var htmlMessage = HtmlMessage.Empty
            .User(request.UserId, request.User.GetFullName())
            .Text(" is banned from Global Ban");

        response.Message = htmlMessage.ToString();
        response.Result = combotAntispamApiDto;

        return response;
    }
}