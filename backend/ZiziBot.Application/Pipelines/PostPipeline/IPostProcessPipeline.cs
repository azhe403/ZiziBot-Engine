namespace ZiziBot.Application.Pipelines.PostPipeline;

public interface IPostProcessPipeline<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task ProcessAsync(TRequest request, TResponse response, CancellationToken cancellationToken);
}
