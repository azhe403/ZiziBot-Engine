using Hangfire;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Tasks;

public class RegisterRssJobTasks : IStartupTask
{
    private readonly ILogger<RegisterRssJobTasks> _logger;
    private readonly ChatDbContext _chatDbContext;
    private readonly IRecurringJobManager _recurringJobManager;

    public bool SkipAwait { get; set; } = true;

    public RegisterRssJobTasks(ILogger<RegisterRssJobTasks> logger, ChatDbContext chatDbContext, IRecurringJobManager recurringJobManager)
    {
        _logger = logger;
        _chatDbContext = chatDbContext;
        _recurringJobManager = recurringJobManager;
    }

    public async Task ExecuteAsync()
    {
        var rssSettings = await _chatDbContext.RssSetting
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .ToListAsync();

        _logger.LogInformation("Registering RSS Jobs. Count: {Count}", rssSettings.Count);

        foreach (var rssSettingEntity in rssSettings)
        {
            var rssId = rssSettingEntity.Id;
            var chatId = rssSettingEntity.ChatId;
            var rssUrl = rssSettingEntity.RssUrl;
            var jobId = $"RssJob:{rssId}";

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
                cronExpression: Cron.Minutely);
        }
    }
}