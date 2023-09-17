using Hangfire;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Telegram.Rss;

public class RegisterRssJobUrlRequest : IRequest<bool>
{
    public long ChatId { get; set; }
    public int ThreadId { get; set; }
    public string Url { get; set; }
    public string JobId { get; set; }
}

public class RegisterRssJobUrlHandler : IRequestHandler<RegisterRssJobUrlRequest, bool>
{
    private readonly ILogger<RegisterRssJobUrlHandler> _logger;
    private readonly IRecurringJobManager _recurringJobManager;

    public RegisterRssJobUrlHandler(ILogger<RegisterRssJobUrlHandler> logger, IRecurringJobManager recurringJobManager)
    {
        _logger = logger;
        _recurringJobManager = recurringJobManager;
    }

    public async Task<bool> Handle(RegisterRssJobUrlRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Registering RSS Job. ChatId: {ChatId}, ThreadId: {ThreadId} RssUrl: {RssUrl}", request.ChatId, request.ThreadId, request.Url);

        _recurringJobManager.RemoveIfExists(request.JobId);
        _recurringJobManager.AddOrUpdate<MediatorService>(
            recurringJobId: request.JobId,
            methodCall: mediatorService => mediatorService.Send(new FetchRssRequest()
            {
                ChatId = request.ChatId,
                ThreadId = request.ThreadId,
                RssUrl = request.Url
            }),
            queue: "rss",
            cronExpression: TimeUtil.MinuteInterval(3));

        return true;
    }
}