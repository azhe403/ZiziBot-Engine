using Hangfire;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Rss;

public class RegisterRssJobRequest : IRequest<bool>
{
    public bool ResetStatus { get; set; }
    public long ChatId { get; set; }
}

public class RegisterRssJobHandler : IRequestHandler<RegisterRssJobRequest, bool>
{
    private readonly ILogger<RegisterRssJobHandler> _logger;
    private readonly ChatDbContext _chatDbContext;
    private readonly IRecurringJobManager _recurringJobManager;

    public RegisterRssJobHandler(ILogger<RegisterRssJobHandler> logger, ChatDbContext chatDbContext, IRecurringJobManager recurringJobManager)
    {
        _logger = logger;
        _chatDbContext = chatDbContext;
        _recurringJobManager = recurringJobManager;
    }

    public async Task<bool> Handle(RegisterRssJobRequest request, CancellationToken cancellationToken)
    {
        if (request.ResetStatus)
        {
            var rssSettingsAll = await _chatDbContext.RssSetting.ToListAsync(cancellationToken: cancellationToken);
            rssSettingsAll.ForEach(entity => entity.Status = (int)EventStatus.Complete);
            await _chatDbContext.SaveChangesAsync(cancellationToken);
        }

        var rssSettings = await _chatDbContext.RssSetting
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .WhereIf(request.ChatId != 0, entity => entity.ChatId == request.ChatId)
            .ToListAsync(cancellationToken: cancellationToken);

        _logger.LogInformation("Registering RSS Jobs. Count: {Count}", rssSettings.Count);

        foreach (var rssSettingEntity in rssSettings)
        {
            var uniqueId = await StringUtil.GetNanoIdAsync(7);
            var rssId = rssSettingEntity.Id;
            var chatId = rssSettingEntity.ChatId;
            var rssUrl = rssSettingEntity.RssUrl;
            var jobId = rssSettingEntity.CronJobId.IsNullOrEmpty() ? $"RssJob:{uniqueId}" : rssSettingEntity.CronJobId;

            _logger.LogDebug("Registering RSS Job. RssId: {RssId}, ChatId: {ChatId}, RssUrl: {RssUrl}", rssId, chatId, rssUrl);

            _recurringJobManager.RemoveIfExists(jobId);
            _recurringJobManager.AddOrUpdate<MediatorService>(
                recurringJobId: jobId,
                methodCall: mediatorService => mediatorService.Send(new FetchRssRequest()
                {
                    ChatId = chatId,
                    RssUrl = rssUrl
                }),
                queue: "rss",
                cronExpression: TimeUtil.MinuteInterval(3));

            rssSettingEntity.CronJobId = jobId;
        }

        await _chatDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}