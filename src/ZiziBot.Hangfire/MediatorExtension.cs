namespace ZiziBot.Hangfire;

public static class MediatorExtension
{
    public static async Task<ResponseBase> RecurringAsync(this IMediator mediator, string jobName, RequestBase request, string cron)
    {
        if (request.DirectAction)
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
        if (request.DirectAction)
        {
            return await mediator.Send(request);
        }

        ResponseBase response = new();
        BackgroundJob.Enqueue<MediatorBridge>(x => x.Send(request));
        return response.Complete();
    }

    public static async Task<ResponseBase> ScheduleAsync(this IMediator mediator, RequestBase request, TimeSpan delay)
    {
        if (request.DirectAction)
        {
            return await mediator.Send(request);
        }

        ResponseBase response = new();
        BackgroundJob.Schedule<MediatorBridge>(x => x.Send(request), delay);
        return response.Complete();
    }
}