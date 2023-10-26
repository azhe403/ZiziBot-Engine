using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Rss;

public class AddRssRequest : BotRequestBase
{

}

public class AddRssHandler : IRequestHandler<AddRssRequest, BotResponseBase>
{
    private readonly TelegramService _telegramService;
    private readonly MongoDbContextBase _mongoDbContext;
    private readonly IMediator _mediator;

    public AddRssHandler(TelegramService telegramService, MongoDbContextBase mongoDbContext, IMediator mediator)
    {
        _telegramService = telegramService;
        _mongoDbContext = mongoDbContext;
        _mediator = mediator;
    }

    public async Task<BotResponseBase> Handle(AddRssRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        if (request.Param.IsNullOrEmpty())
        {
            return await _telegramService.SendMessageAsync("Masukkan RSS URL yang ingin ditambahkan");
        }

        var rssUrl = request.Param.TryFixRssUrl();

        try
        {
            await _telegramService.SendMessageAsync("Sedang memverifikasi URL");
            var feed = await rssUrl.ReadRssAsync();
        }
        catch (Exception e)
        {
            return await _telegramService.SendMessageAsync("Sepertinya bukan URL yang valid");
        }

        var rssSetting = await _mongoDbContext.RssSetting
            .Where(entity => entity.RssUrl == rssUrl)
            .Where(entity => entity.ChatId == request.ChatIdentifier)
            .Where(entity => entity.ThreadId == request.MessageThreadId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (rssSetting != null)
            return await _telegramService.SendMessageAsync("RSS Sudah disimpan");

        var uniqueId = await StringUtil.GetNanoIdAsync(prefix: "RssJob:", size: 7);

        _mongoDbContext.RssSetting.Add(new()
        {
            ChatId = request.ChatIdentifier,
            RssUrl = rssUrl,
            ThreadId = request.MessageThreadId,
            UserId = request.UserId,
            CronJobId = uniqueId,
            Status = (int)EventStatus.Complete
        });

        await _mongoDbContext.SaveChangesAsync(cancellationToken);

        await _telegramService.SendMessageAsync("Membuat Cron Job");

        await _mediator.Send(new RegisterRssJobUrlRequest
        {
            ChatId = request.ChatIdentifier,
            ThreadId = request.MessageThreadId,
            Url = rssUrl,
            JobId = uniqueId
        }, cancellationToken);

        return await _telegramService.SendMessageAsync("RSS Berhasil disimpan");

    }
}