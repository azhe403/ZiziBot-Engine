using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Core;

public class LoggingPipelineBehaviour<TRequest, TResponse>(ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse> where TResponse : new()
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var sessionId = Guid.NewGuid().ToString();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            logger.LogDebug("SessionId: {SessionId}: Handling request of type {@RequestType}", sessionId, typeof(TRequest));
            var result = await next();
            logger.LogDebug("SessionId: {SessionId}: Handled request of type {@RequestType} complete in {Elapsed} ms", sessionId, typeof(TRequest), stopwatch.Elapsed);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error when handling. Message: {Message}", ex.Message);
            return new TResponse();
        }
    }
}