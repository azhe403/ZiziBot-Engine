using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Pipelines.PostPipeline;

public class PostProcessPipeline<TRequest, TResponse>(
    IEnumerable<IPostProcessPipeline<TRequest, TResponse>> postProcessors,
    ILogger<PostProcessPipeline<TRequest, TResponse>> logger
) : IQueryPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, QueryHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var sessionId = PipelineMetrics.CurrentSessionId ?? "none";
        var requestType = PipelineMetrics.GetTypeName(typeof(TRequest));
        var processorList = postProcessors.ToList();
        var response = await next();
        var stageStopwatch = Stopwatch.StartNew();

        logger.LogDebug(
            "PipelineMetrics Stage={Stage} SessionId={SessionId} RequestType={RequestType} ProcessorCount={ProcessorCount} Duration={DurationMs} Status={Status}",
            "Post",
            sessionId,
            requestType,
            processorList.Count,
            0,
            "Started"
        );

        foreach (var (postProcessor, index) in processorList.Select((processor, processorIndex) => (processor, processorIndex + 1)))
        {
            var processorType = PipelineMetrics.GetTypeName(postProcessor.GetType());
            var processorStopwatch = Stopwatch.StartNew();
            await postProcessor.ProcessAsync(request, response, cancellationToken);
            processorStopwatch.Stop();

            logger.LogInformation(
                "PipelineMetrics Stage={Stage} SessionId={SessionId} RequestType={RequestType} Pipeline={Pipeline} Index={Index} Duration={DurationMs} Status={Status}",
                "Post",
                sessionId,
                requestType,
                processorType,
                index,
                processorStopwatch.Elapsed.ToString("mm\\:ss\\.fff"),
                "Completed"
            );
        }

        stageStopwatch.Stop();
        logger.LogInformation(
            "PipelineMetrics Stage={Stage} SessionId={SessionId} RequestType={RequestType} ProcessorCount={ProcessorCount} Duration={DurationMs} Status={Status}",
            "Post",
            sessionId,
            requestType,
            processorList.Count,
            stageStopwatch.Elapsed.ToString("mm\\:ss\\.fff"),
            "Completed"
        );

        return response;
    }
}
