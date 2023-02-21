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

        var stackFrame = exception.ToStackTrace()
            .GetFrames()
            .FirstOrDefault(frame => frame.GetFileLineNumber() > 0);

        if (EventLogConfig.ChatId == 0)
        {
            _logger.LogWarning("EventLogConfig.ChatId is not set");
            return;
        }

        _telegramService.ChatId = EventLogConfig.ChatId;

        var htmlMessage = HtmlMessage.Empty
            .BoldBr("🛑 Exception Handler")
            .Bold("Message: ").CodeBr(exception.Message)
            .Bold("Source: ").CodeBr(exception.Source ?? "N/A")
            .Bold("Type: ").CodeBr(exception.GetType().Name)
            .Bold("Exception: ").CodeBr(typeof(TException).Name)
            .Bold("Request: ").CodeBr(typeof(TRequest).Name);

        if (stackFrame != null)
        {
            htmlMessage
                .Bold("File: ").CodeBr(stackFrame.GetFileName()!)
                .Bold("Coordinate: ").CodeBr($"{stackFrame.GetFileLineNumber()}:{stackFrame.GetFileColumnNumber()}")
                .Bold("Namespace: ").CodeBr(stackFrame.GetMethod()!.DeclaringType!.Namespace!)
                .Bold("Assembly: ").CodeBr(stackFrame.GetMethod()!.DeclaringType!.Assembly.GetName().Name ?? string.Empty)
                .Bold("Assembly Version: ").CodeBr(stackFrame.GetMethod()!.DeclaringType!.Assembly.GetName().Version!.ToString())
                .Bold("Assembly Location: ").CodeBr(stackFrame.GetMethod()!.DeclaringType!.Assembly.Location);
        }

        await _telegramService.SendMessageText(htmlMessage.ToString());

        state.SetHandled(default!);
    }
}