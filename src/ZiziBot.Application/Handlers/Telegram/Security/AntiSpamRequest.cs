using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Telegram.Security;

public class AntiSpamRequestModel : RequestBase
{
}

public class AntiSpamPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<AntiSpamPipelineBehaviour<TRequest, TResponse>> _logger;
    private readonly AntiSpamService _antiSpamService;

    public AntiSpamPipelineBehaviour(ILogger<AntiSpamPipelineBehaviour<TRequest, TResponse>> logger, AntiSpamService antiSpamService)
    {
        _logger = logger;
        _antiSpamService = antiSpamService;
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

        requestBase.ReplyMessage = false;
        ResponseBase responseBase = new(requestBase);

        var combotAntispamApiDto = await _antiSpamService.CheckCombotAntiSpamAsync(requestBase.ChatIdentifier, requestBase.UserId);

        if (combotAntispamApiDto.Result != null)
        {
            await responseBase.SendMessageText("User is banned from Global Ban");
            await responseBase.DeleteMessageAsync();
            return default!;
        }


        return await next();
    }
}