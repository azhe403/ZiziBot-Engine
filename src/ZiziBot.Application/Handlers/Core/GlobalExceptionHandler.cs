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

    private EventLogConfig EventLogConfig => _eventLogOptions.Value;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler<TRequest, TResponse, TException>> logger, IOptions<EventLogConfig> eventLogOptions)
    {
        _logger = logger;
        _eventLogOptions = eventLogOptions;
    }

    public async Task Handle(
        TRequest request,
        TException exception,
        RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken
    )
    {
        ResponseBase responseBase = new(request);

        _logger.LogError(exception, "Something went wrong while handling request of type {@requestType}", typeof(TRequest));

        var stackFrame = exception.ToStackTrace()
            .GetFrames()
            .FirstOrDefault(frame => frame.GetFileLineNumber() > 0);

        if (EventLogConfig.ChatId == 0)
        {
            _logger.LogWarning("EventLogConfig.ChatId is not set");
            return;
        }

        responseBase.ChatId = EventLogConfig.ChatId;

        var htmlMessage = HtmlMessage.Empty
            .BoldBr("Exception Handler")
            .Bold("Message: ").TextBr(exception.Message)
            .Bold("Source: ").TextBr(exception.Source ?? "N/A")
            .Bold("Type: ").TextBr(exception.GetType().Name)
            .Bold("Exception: ").TextBr(typeof(TException).Name)
            .Bold("Request: ").TextBr(typeof(TRequest).Name);

        if (stackFrame != null)
        {
            htmlMessage
                .Bold("File: ").TextBr(stackFrame.GetFileName()!)
                .Bold("Coordinate: ").TextBr($"{stackFrame.GetFileLineNumber()}:{stackFrame.GetFileColumnNumber()}")
                .Bold("Namespace: ").TextBr(stackFrame.GetMethod()!.DeclaringType!.Namespace!)
                .Bold("Assembly: ").TextBr(stackFrame.GetMethod()!.DeclaringType!.Assembly.GetName().Name ?? string.Empty)
                .Bold("Assembly Version: ").TextBr(stackFrame.GetMethod()!.DeclaringType!.Assembly.GetName().Version!.ToString())
                .Bold("Assembly Location: ").TextBr(stackFrame.GetMethod()!.DeclaringType!.Assembly.Location);
        }

        await responseBase.SendMessageText(htmlMessage.ToString());

        state.SetHandled(default!);
    }
}