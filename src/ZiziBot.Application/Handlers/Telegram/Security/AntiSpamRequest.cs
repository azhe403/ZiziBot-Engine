using System.Security;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Telegram.Security;

public class AntiSpamRequestModel : RequestBase
{
}

public class AntiSpamPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<AntiSpamPipelineBehaviour<TRequest, TResponse>> _logger;
    private readonly AntiSpamService _antiSpamService;
    private readonly TelegramService _telegramService;

    public AntiSpamPipelineBehaviour(ILogger<AntiSpamPipelineBehaviour<TRequest, TResponse>> logger, AntiSpamService antiSpamService, TelegramService telegramService)
    {
        _logger = logger;
        _antiSpamService = antiSpamService;
        _telegramService = telegramService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestBase = request as RequestBase;
        if (requestBase == null)
            return await next();

        var ignoreTypes = new List<Type>
        {
            typeof(DeleteMessageRequestModel)
        };

        if (ignoreTypes.Contains(typeof(TRequest)))
        {
            _logger.LogDebug("Ignoring request of type {@requestType} because should be ignored", typeof(TRequest));
            return await next();
        }

        _telegramService.SetupResponse(requestBase);

        var combotAntispamApiDto = await _antiSpamService.CheckSpamAsync(requestBase.ChatIdentifier, requestBase.UserId);

        if (!combotAntispamApiDto.IsBanAny)
            return await next();

        var htmlMessage = HtmlMessage.Empty
            .User(requestBase.UserId, requestBase.UserFullName)
            .Text(" is banned from Global Ban");

        await _telegramService.DeleteMessageAsync();
        await _telegramService.SendMessageText(htmlMessage.ToString());

        throw new SecurityException($"UserId: {requestBase.UserId} is marked as spammer");
    }
}