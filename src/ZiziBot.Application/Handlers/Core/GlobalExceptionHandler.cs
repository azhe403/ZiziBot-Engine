using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZiziBot.Application.Handlers.Core;

public class GlobalExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : RequestBase, IRequest<TResponse>
    where TException : Exception
{
    private readonly ILogger<GlobalExceptionHandler<TRequest, TResponse, TException>> _logger;
    private readonly IOptions<EventLogConfig> _eventLogOptions;
    private readonly TelegramService _telegramService;

    private EventLogConfig EventLogConfig => _eventLogOptions.Value;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler<TRequest, TResponse, TException>> logger,
        IOptions<EventLogConfig> eventLogOptions,
        TelegramService telegramService
    )
    {
        _logger = logger;
        _eventLogOptions = eventLogOptions;
        _telegramService = telegramService;
    }

    public async Task Handle(
        TRequest request,
        TException exception,
        RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken
    )
    {
        _telegramService.SetupResponse(request);

        _logger.LogError(exception, "Something went wrong while handling request of type {@requestType}", typeof(TRequest));

        await Task.Delay(1, cancellationToken);

        state.SetHandled(default!);
    }
}