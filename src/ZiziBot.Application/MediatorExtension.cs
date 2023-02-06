using Hangfire;
using Serilog;

namespace ZiziBot.Application;

public static class MediatorExtension
{
    public static async Task<ResponseBase> RecurringAsync(
        this IMediator mediator,
        string jobName,
        RequestBase request,
        string cron
    )
    {
        if (request.ExecutionStrategy == ExecutionStrategy.Instant)
        {
            return await mediator.Send(request);
        }

        ResponseBase response = new();
        RecurringJob.AddOrUpdate<MediatorBridge>(jobName, x => x.Send(request), cron);
        return response.Complete();
    }

    [Obsolete("Please use EnqueueAsync")]
    public static void Enqueue(this IMediator mediator, RequestBase request)
    {
        BackgroundJob.Enqueue<MediatorBridge>(x => x.Send(request));
    }

    public static async Task<ResponseBase> EnqueueAsync(this IMediator mediator, RequestBase request)
    {
        Log.Debug("Enqueueing request {request} in {Mode}", request, request.ExecutionStrategy);

        if (request.ExecutionStrategy == ExecutionStrategy.Instant)
        {
            return await mediator.Send(request);
        }

        ResponseBase response = new();

        var enqueue = request.ExecutionStrategy switch
        {
            ExecutionStrategy.Hangfire => BackgroundJob.Enqueue<MediatorBridge>(x => x.Send(request)),
            _ => throw new ArgumentOutOfRangeException(nameof(request.ExecutionStrategy), request.ExecutionStrategy, null)
        };

        return response.Complete();
    }

    public static ResponseBase Schedule(this IMediator mediator, RequestBase request)
    {
        ResponseBase response = new();
        BackgroundJob.Schedule<MediatorBridge>(x => x.Send(request), request.DeleteAfter);
        return response.Complete();
    }
}