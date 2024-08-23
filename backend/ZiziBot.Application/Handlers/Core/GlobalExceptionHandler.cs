using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Core;

public class GlobalExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : BotRequestBase
    where TException : Exception
{
    private readonly ILogger<GlobalExceptionHandler<TRequest, TResponse, TException>> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler<TRequest, TResponse, TException>> logger
    )
    {
        _logger = logger;
    }

    public async Task Handle(
        TRequest request,
        TException exception,
        RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken
    )
    {
        _logger.LogError(exception, "Exception thrown when handling Request: {@Request}", typeof(TRequest));

        await Task.Delay(1, cancellationToken);

        state.SetHandled(default!);
    }
}