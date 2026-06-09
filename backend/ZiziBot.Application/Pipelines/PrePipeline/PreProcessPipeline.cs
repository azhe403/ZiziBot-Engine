using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Pipelines.PrePipeline;

public class PreProcessPipeline<TRequest, TResponse>(
    IEnumerable<IPreProcessPipeline<TRequest, TResponse>> preProcessors,
    ILogger<PreProcessPipeline<TRequest, TResponse>> logger
) : IQueryPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        QueryHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var sessionId = PipelineMetrics.CurrentSessionId ?? "none";
        var requestType = PipelineMetrics.GetTypeName(typeof(TRequest));
        var processorList = preProcessors.ToList();
        var stageStopwatch = Stopwatch.StartNew();

        logger.LogDebug(
            "PipelineMetrics Stage={Stage} SessionId={SessionId} RequestType={RequestType} ProcessorCount={ProcessorCount} Duration={DurationMs} Status={Status}",
            "Pre",
            sessionId,
            requestType,
            processorList.Count,
            0,
            "Started"
        );

        foreach (var (preProcessor, index) in processorList.Select((processor, processorIndex) =>
                     (processor, processorIndex + 1)))
        {
            var processorType = PipelineMetrics.GetTypeName(preProcessor.GetType());
            var processorStopwatch = Stopwatch.StartNew();
            var result = await preProcessor.ProcessAsync(request, cancellationToken);
            processorStopwatch.Stop();

            logger.LogInformation(
                "PipelineMetrics Stage={Stage} SessionId={SessionId} RequestType={RequestType} Pipeline={Pipeline} Index={Index} Duration={DurationMs} ShouldContinue={ShouldContinue} Status={Status}",
                "Pre",
                sessionId,
                requestType,
                processorType,
                index,
                processorStopwatch.Elapsed.ToString("mm\\:ss\\.fff"),
                result.ShouldContinue,
                result.ShouldContinue ? "Completed" : "ShortCircuited"
            );

            if (!result.ShouldContinue)
            {
                stageStopwatch.Stop();
                logger.LogInformation(
                    "PipelineMetrics Stage={Stage} SessionId={SessionId} RequestType={RequestType} ProcessorCount={ProcessorCount} Duration={Duration} Status={Status}",
                    "Pre",
                    sessionId,
                    requestType,
                    processorList.Count,
                    stageStopwatch.Elapsed.ToString("mm\\:ss\\.fff"),
                    "ShortCircuited"
                );

                return result.Response!;
            }
        }

        stageStopwatch.Stop();
        logger.LogInformation(
            "PipelineMetrics Stage={Stage} SessionId={SessionId} RequestType={RequestType} ProcessorCount={ProcessorCount} Duration={Duration} Status={Status}",
            "Pre",
            sessionId,
            requestType,
            processorList.Count,
            stageStopwatch.Elapsed.ToString("mm\\:ss\\.fff"),
            "Completed"
        );

        return await next();
    }
}
