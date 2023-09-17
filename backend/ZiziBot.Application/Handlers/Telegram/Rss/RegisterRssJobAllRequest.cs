using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Rss;

public class RegisterRssJobAllRequest : IRequest<bool>
{
    public bool ResetStatus { get; set; }
    public long ChatId { get; set; }
}

public class RegisterRssJobAllHandler : IRequestHandler<RegisterRssJobAllRequest, bool>
{
    private readonly ILogger<RegisterRssJobAllHandler> _logger;
    private readonly MongoDbContextBase _mongoDbContext;
    private readonly IMediator _mediator;

    public RegisterRssJobAllHandler(ILogger<RegisterRssJobAllHandler> logger, MongoDbContextBase mongoDbContext, IMediator mediator)
    {
        _logger = logger;
        _mongoDbContext = mongoDbContext;
        _mediator = mediator;
    }

    public async Task<bool> Handle(RegisterRssJobAllRequest request, CancellationToken cancellationToken)
    {
        if (request.ResetStatus)
        {
            var rssSettingsAll = await _mongoDbContext.RssSetting.ToListAsync(cancellationToken: cancellationToken);
            rssSettingsAll.ForEach(entity => {
                entity.LastErrorMessage = string.Empty;
                entity.Status = (int)EventStatus.Complete;
            });
            await _mongoDbContext.SaveChangesAsync(cancellationToken);
        }

        var rssSettings = await _mongoDbContext.RssSetting
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .WhereIf(request.ChatId != 0, entity => entity.ChatId == request.ChatId)
            .ToListAsync(cancellationToken: cancellationToken);

        _logger.LogInformation("Registering RSS Jobs. Count: {Count}", rssSettings.Count);

        foreach (var rssSettingEntity in rssSettings)
        {
            var uniqueId = await StringUtil.GetNanoIdAsync(prefix: "RssJob:", size: 7);
            var jobId = rssSettingEntity.CronJobId.IsNullOrEmpty() ? uniqueId : rssSettingEntity.CronJobId;

            await _mediator.Send(new RegisterRssJobUrlRequest
            {
                ChatId = rssSettingEntity.ChatId,
                ThreadId = rssSettingEntity.ThreadId,
                Url = rssSettingEntity.RssUrl,
                JobId = jobId
            });

            rssSettingEntity.CronJobId = jobId;
        }

        await _mongoDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}