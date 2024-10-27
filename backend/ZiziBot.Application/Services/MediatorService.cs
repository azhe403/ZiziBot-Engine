using System.ComponentModel;
using CloudCraic.Hosting.BackgroundQueue;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Services;

public class MediatorService(
    ILogger<MediatorService> logger,
    IMediator mediator,
    IBackgroundQueue backgroundQueue
)
{
    #region Execution
    public async Task<BotResponseBase> EnqueueAsync(BotRequestBase request)
    {
        BotResponseBase botResponse = new();
        logger.LogDebug("Enqueueing request {Request} in {Mode}", request, request.ExecutionStrategy);

        switch (request.ExecutionStrategy)
        {
            case ExecutionStrategy.Hangfire:
                var jobId = $"{request.Source}-{request.ChatId}-{request.MessageId}-{request.UserId}-{request.Command}";
                BackgroundJob.Enqueue<MediatorService>(x => x.Send(jobId, request));

                break;
            case ExecutionStrategy.Background:
                backgroundQueue.Enqueue(async token => await mediator.Send(request, token));

                break;
            case ExecutionStrategy.Instant:
                return await mediator.Send(request);
            default:
                throw new ArgumentOutOfRangeException(nameof(request.ExecutionStrategy), request.ExecutionStrategy, "Unknown execution strategy");
        }

        return botResponse.Complete();
    }

    public async Task<ApiResponseBase<T>> EnqueueAsync<T>(ApiRequestBase<T> request, ExecutionStrategy executionStrategy = default)
    {
        switch (executionStrategy)
        {
            case ExecutionStrategy.Hangfire:
                var jobId = $"{request.HttpContextAccessor?.HttpContext?.TraceIdentifier}";
                BackgroundJob.Enqueue<MediatorService>(x => x.Send(jobId, request));

                return new();

                break;

            case ExecutionStrategy.Instant:
                return await mediator.Send(request);
            default:
                throw new ArgumentOutOfRangeException(nameof(executionStrategy));
        }
    }

    public async Task<T> EnqueueAsync<T>(IRequest<T> request, ExecutionStrategy executionStrategy = default) where T : new()
    {
        switch (executionStrategy)
        {
            case ExecutionStrategy.Hangfire:
                BackgroundJob.Enqueue<MediatorService>(x => x.Send(request));

                return new();

                break;

            case ExecutionStrategy.Instant:
                return await mediator.Send(request);
            default:
                throw new ArgumentOutOfRangeException(nameof(executionStrategy));
        }
    }

    public BotResponseBase Schedule(BotRequestBase request, TimeSpan delayExecution = default)
    {
        BotResponseBase botResponse = new();
        BackgroundJob.Schedule<MediatorService>(x => x.Send(request), delayExecution);

        return botResponse.Complete();
    }
    #endregion

    #region Bridge
    [DisplayName("{0}")]
    [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Delete, Attempts = 3)]
    public async Task<TResponse?> Send<TResponse>(IRequest<TResponse> request)
    {
        return await mediator.Send(request);
    }

    [DisplayName("{0}")]
    [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Delete, Attempts = 3)]
    public async Task<TResponse?> Send<TResponse>(string jobName, IRequest<TResponse> request)
    {
        return await mediator.Send(request);
    }
    #endregion
}