using Hangfire;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.Application.Handlers.Telegram.Rss;

public class FetchRssRequest : IRequest<bool>
{
    public long ChatId { get; set; }
    public string RssUrl { get; set; }
}

public class FetchRssHandler : IRequestHandler<FetchRssRequest, bool>
{
    private readonly ILogger<FetchRssHandler> _logger;
    private readonly AppSettingsDbContext _appSettingsDbContext;
    private readonly ChatDbContext _chatDbContext;
    private readonly IRecurringJobManager _recurringJobManager;

    public FetchRssHandler(ILogger<FetchRssHandler> logger, AppSettingsDbContext appSettingsDbContext, ChatDbContext chatDbContext, IRecurringJobManager recurringJobManager)
    {
        _logger = logger;
        _appSettingsDbContext = appSettingsDbContext;
        _chatDbContext = chatDbContext;
        _recurringJobManager = recurringJobManager;
    }

    public async Task<bool> Handle(FetchRssRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing RSS Url: {Url}", request.RssUrl);

        try
        {
            var feed = await request.RssUrl.ReadRssAsync();

            var latestArticle = feed.Items.FirstOrDefault();

            if (latestArticle == null)
            {
                _logger.LogInformation("No article found in ChatId: {ChatId} for RSS Url: {Url}", request.ChatId, request.RssUrl);
                return false;
            }

            var latestHistory = await _chatDbContext.RssHistory
                .FirstOrDefaultAsync(history =>
                        history.ChatId == request.ChatId &&
                        history.RssUrl == request.RssUrl &&
                        history.Status == (int)EventStatus.Complete,
                    cancellationToken: cancellationToken);

            if (latestHistory != null)
            {
                _logger.LogDebug("No new article found in ChatId: {ChatId} for RSS Url: {Url}", request.ChatId, request.RssUrl);
                return false;
            }

            var botSettings = await _appSettingsDbContext.BotSettings
                .FirstOrDefaultAsync(settings =>
                        settings.Name == "Main" &&
                        settings.Status == (int)EventStatus.Complete,
                    cancellationToken: cancellationToken);

            var telegramBotClient = new TelegramBotClient(botSettings.Token);

            await telegramBotClient.SendTextMessageAsync(
                chatId: request.ChatId,
                text: $"{latestArticle.Title}\n{latestArticle.Link}",
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);

            _chatDbContext.RssHistory.Add(new RssHistoryEntity()
            {
                ChatId = request.ChatId,
                RssUrl = request.RssUrl,
                Title = latestArticle.Title,
                Url = latestArticle.Link,
                Author = latestArticle.Author,
                PublishDate = latestArticle.PublishingDate.GetValueOrDefault(DateTime.Now),
                Status = (int)EventStatus.Complete
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while sending RSS article to Chat: {ChatId}. Url: {Url}", request.ChatId, request.RssUrl);

            if (exception.Message.IsIgnorable())
            {
                _logger.LogWarning("Disabling and remove in ChatId: {ChatId} for RSS Url: {Url}", request.ChatId, request.RssUrl);

                var rssSetting = await _chatDbContext.RssSetting
                    .FirstOrDefaultAsync(entity =>
                            entity.ChatId == request.ChatId &&
                            entity.RssUrl == request.RssUrl,
                        cancellationToken: cancellationToken);

                if (rssSetting != null)
                {
                    rssSetting.Status = (int)EventStatus.InProgress;
                    rssSetting.LastErrorMessage = exception.Message;

                    var jobId = "RssJob:" + rssSetting.Id;
                    _recurringJobManager.RemoveIfExists(jobId);
                }
            }
        }

        await _chatDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}