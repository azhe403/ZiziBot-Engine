using Hangfire;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Tasks;

public class RegisterRssJobTasks : IStartupTask
{
    private readonly ILogger<RegisterRssJobTasks> _logger;
    private readonly MediatorService _mediatorService;
    private readonly ChatDbContext _chatDbContext;

    public bool SkipAwait { get; set; } = true;

    public RegisterRssJobTasks(ILogger<RegisterRssJobTasks> logger, MediatorService mediatorService, ChatDbContext chatDbContext)
    {
        _logger = logger;
        _mediatorService = mediatorService;
        _chatDbContext = chatDbContext;
    }

    public async Task ExecuteAsync()
    {
        // await _mediatorService.Send(new RegisterRssJobRequest()
        // {
        //     ResetStatus = true
        // });

        // if (request.ResetStatus)
        // {
        //     var rssSettingsAll = await _chatDbContext.RssSetting.ToListAsync(cancellationToken: cancellationToken);
        //     rssSettingsAll.ForEach(entity => entity.Status = (int)EventStatus.Complete);
        //     await _chatDbContext.SaveChangesAsync(cancellationToken);
        // }

        var rssSettings = await _chatDbContext.RssSetting
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .ToListAsync();

        _logger.LogInformation("Registering RSS Jobs. Count: {Count}", rssSettings.Count);

        foreach (var rssSettingEntity in rssSettings)
        {
            var rssId = rssSettingEntity.Id;
            var chatId = rssSettingEntity.ChatId;
            var rssUrl = rssSettingEntity.RssUrl;
            var jobId = "RssJob:" + rssSettingEntity.Id;

            _logger.LogDebug("Registering RSS Job. RssId: {RssId}, ChatId: {ChatId}, RssUrl: {RssUrl}", rssId, chatId, rssUrl);

            RecurringJob.AddOrUpdate<MediatorService>(
                recurringJobId: jobId,
                methodCall: mediatorService => mediatorService.Send(new FetchRssRequest()
                {
                    ChatId = chatId,
                    RssUrl = rssUrl
                }),
                queue: "rss",
                cronExpression: TimeUtil.MinuteInterval(3));
        }
    }
}