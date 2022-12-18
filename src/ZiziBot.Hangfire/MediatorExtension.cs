namespace ZiziBot.Hangfire;

public static class MediatorExtension
{
    public static void Recurring(this IMediator mediator, string jobName, IRequest<string> request, string cron)
    {
        RecurringJob.AddOrUpdate<MediatorBridge>(jobName, x => x.Send(request), cron);
    }

    public static void Enqueue<T>(this IMediator mediator, IRequest<T> request)
    {
        BackgroundJob.Enqueue<MediatorBridge>(x => x.Send(request));
    }

    public static void Schedule<T>(this IMediator mediator, IRequest<T> request, TimeSpan delay)
    {
        BackgroundJob.Schedule<MediatorBridge>(x => x.Send(request), delay);
    }
}