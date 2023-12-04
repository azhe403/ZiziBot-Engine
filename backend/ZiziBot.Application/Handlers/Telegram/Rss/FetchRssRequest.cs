using Hangfire;
using Humanizer;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;
using MoreLinq;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.Application.Handlers.Telegram.Rss;

public class FetchRssRequest : IRequest<bool>
{
    public long ChatId { get; set; }
    public int ThreadId { get; set; }
    public string RssUrl { get; set; }
}

public class FetchRssHandler : IRequestHandler<FetchRssRequest, bool>
{
    private readonly ILogger<FetchRssHandler> _logger;
    private readonly MongoDbContextBase _mongoDbContext;
    private readonly AppSettingRepository _appSettingRepository;

    public FetchRssHandler(ILogger<FetchRssHandler> logger, MongoDbContextBase mongoDbContext, AppSettingRepository appSettingRepository)
    {
        _logger = logger;
        _mongoDbContext = mongoDbContext;
        _appSettingRepository = appSettingRepository;
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

            var latestHistory = await _mongoDbContext.RssHistory
                .FirstOrDefaultAsync(history =>
                        history.ChatId == request.ChatId &&
                        history.ThreadId == request.ThreadId &&
                        history.RssUrl == request.RssUrl &&
                        history.Status == (int)EventStatus.Complete,
                    cancellationToken: cancellationToken);

            if (latestHistory != null)
            {
                _logger.LogDebug("No new article found in ChatId: {ChatId} for RSS Url: {Url}", request.ChatId, request.RssUrl);
                return false;
            }

            var botSettings = await _appSettingRepository.GetBotMain();

            var botClient = new TelegramBotClient(botSettings.Token);

            var htmlContent = await latestArticle.Content.HtmlForTelegram();

            var messageText = HtmlMessage.Empty
                .Url(feed.Link, feed.Title).Br()
                .Url(latestArticle.Link, latestArticle.Title).Br()
                .Text(htmlContent.Truncate(2000));

            if (request.RssUrl.IsGithubReleaseUrl())
            {
                var assets = await request.RssUrl.GetGithubAssetLatest();


                if (assets != null)
                {
                    messageText.Br().Br()
                        .BoldBr("Assets");

                    assets.Assets.ForEach(asset => {
                        messageText.Url(asset.Url, asset.Name).Br();
                    });
                }
            }

            var truncatedMessageText = messageText.ToString();

            try
            {
                await botClient.SendTextMessageAsync(
                    chatId: request.ChatId,
                    messageThreadId: request.ThreadId,
                    text: truncatedMessageText,
                    parseMode: ParseMode.Html,
                    disableWebPagePreview: true,
                    cancellationToken: cancellationToken
                );
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Trying send RSS without thread to ChatId: {ChatId}", request.ChatId);
                if (exception.Message.Contains("thread not found"))
                {
                    await botClient.SendTextMessageAsync(
                        chatId: request.ChatId,
                        text: truncatedMessageText,
                        parseMode: ParseMode.Html,
                        disableWebPagePreview: true,
                        cancellationToken: cancellationToken
                    );
                }
            }

            _mongoDbContext.RssHistory.Add(new RssHistoryEntity()
            {
                ChatId = request.ChatId,
                ThreadId = request.ThreadId,
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
            if (exception.Message.IsIgnorable())
            {
                var findRssSetting = await _mongoDbContext.RssSetting
                    .Where(entity => entity.ChatId == request.ChatId)
                    .Where(entity => entity.RssUrl == request.RssUrl)
                    .Where(entity => entity.Status == (int)EventStatus.Complete)
                    .ToListAsync(cancellationToken);

                findRssSetting.ForEach(e => {
                    _logger.LogWarning("Removing RSS CronJob for ChatId: {ChatId}, Url: {Url}", e.ChatId, e.RssUrl);

                    e.Status = (int)EventStatus.InProgress;
                    e.LastErrorMessage = exception.Message;

                    RecurringJob.RemoveIfExists(e.CronJobId);
                });
            }
            else
            {
                _logger.LogError(exception, "Error while sending RSS article to Chat: {ChatId}. Url: {Url}", request.ChatId, request.RssUrl);
            }
        }

        await _mongoDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}