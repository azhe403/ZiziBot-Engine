using Hangfire;

namespace ZiziBot.Application.Handlers.Telegram.Rss;

public class RegisterRssJobUrlRequest : IRequest<bool>
{
    public long ChatId { get; set; }
    public string Url { get; set; }
    public string JobId { get; set; }
}

public class RegisterRssJobUrlHandler : IRequestHandler<RegisterRssJobUrlRequest, bool>
{
    private readonly IRecurringJobManager _recurringJobManager;

    public RegisterRssJobUrlHandler(IRecurringJobManager recurringJobManager)
    {
        _recurringJobManager = recurringJobManager;
    }

    public async Task<bool> Handle(RegisterRssJobUrlRequest request, CancellationToken cancellationToken)
    {
        _recurringJobManager.RemoveIfExists(request.JobId);
        _recurringJobManager.AddOrUpdate<MediatorService>(
            recurringJobId: request.JobId,
            methodCall: mediatorService => mediatorService.Send(new FetchRssRequest()
            {
                ChatId = request.ChatId,
                RssUrl = request.Url
            }),
            queue: "rss",
            cronExpression: TimeUtil.MinuteInterval(3));

        return true;
    }
}