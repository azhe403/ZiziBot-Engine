namespace ZiziBot.Application.Pipelines.PrePipeline;

public interface IPreProcessPipeline<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<PreProcessResult<TResponse>> ProcessAsync(TRequest request, CancellationToken cancellationToken);
}

public sealed record PreProcessResult<TResponse>(bool ShouldContinue, TResponse? Response = default)
{
    public static PreProcessResult<TResponse> Continue { get; } = new(true);

    public static PreProcessResult<TResponse> Stop(TResponse response)
    {
        return new(false, response);
    }

    public static PreProcessResult<TResponse> Stop()
    {
        return new(false);
    }
}
