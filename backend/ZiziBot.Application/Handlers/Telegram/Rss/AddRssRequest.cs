using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Rss;

public class AddRssRequest : BotRequestBase
{

}

public class AddRssHandler : IRequestHandler<AddRssRequest, BotResponseBase>
{
    private readonly TelegramService _telegramService;
    private readonly MongoDbContextBase _mongoDbContext;

    public AddRssHandler(TelegramService telegramService, MongoDbContextBase mongoDbContext)
    {
        _telegramService = telegramService;
        _mongoDbContext = mongoDbContext;
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

        _mongoDbContext.RssSetting.Add(new()
        {
            ChatId = request.ChatIdentifier,
            RssUrl = rssUrl,
            ThreadId = request.MessageThreadId,
            UserId = request.UserId,
            Status = (int)EventStatus.Complete
        });

        await _mongoDbContext.SaveChangesAsync(cancellationToken);

        return await _telegramService.SendMessageAsync("RSS Berhasil disimpan");

    }
}