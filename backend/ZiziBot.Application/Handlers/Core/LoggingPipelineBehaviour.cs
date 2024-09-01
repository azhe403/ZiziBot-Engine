using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Core;

public class LoggingPipelineBehaviour<TRequest, TResponse>(ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var sessionId = Guid.NewGuid().ToString();
        var stopwatch = Stopwatch.StartNew();

        logger.LogDebug("SessionId: {SessionId}: Handling request of type {@requestType}", sessionId, typeof(TRequest));
        var result = await next();
        logger.LogDebug("SessionId: {SessionId}: Handled request of type {@requestType} complete in {elapsed} ms", sessionId, typeof(TRequest),
            stopwatch.Elapsed);

        return result;
    }
}