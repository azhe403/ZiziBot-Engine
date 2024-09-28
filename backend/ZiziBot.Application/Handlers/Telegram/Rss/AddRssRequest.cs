using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Rss;

public class AddRssRequest : BotRequestBase
{

}

public class AddRssHandler(TelegramService telegramService, MongoDbContextBase mongoDbContext, IMediator mediator) : IRequestHandler<AddRssRequest, BotResponseBase>
{
    public async Task<BotResponseBase> Handle(AddRssRequest request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        if (request.Param.IsNullOrEmpty())
        {
            return await telegramService.SendMessageAsync("Masukkan RSS URL yang ingin ditambahkan");
        }

        var rssUrl = request.Param.TryFixRssUrl();

        try
        {
            await telegramService.SendMessageAsync("Sedang memverifikasi URL");
            var feed = await rssUrl.ReadRssAsync();
        }
        catch (Exception e)
        {
            return await telegramService.SendMessageAsync("Sepertinya bukan URL yang valid");
        }

        var rssSetting = await mongoDbContext.RssSetting
            .Where(entity => entity.RssUrl == rssUrl)
            .Where(entity => entity.ChatId == request.ChatIdentifier)
            .Where(entity => entity.ThreadId == request.MessageThreadId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (rssSetting != null)
            return await telegramService.SendMessageAsync("RSS Sudah disimpan");

        var uniqueId = await StringUtil.GetNanoIdAsync(prefix: "RssJob:", size: 7);

        mongoDbContext.RssSetting.Add(new()
        {
            ChatId = request.ChatIdentifier,
            RssUrl = rssUrl,
            ThreadId = request.MessageThreadId,
            UserId = request.UserId,
            CronJobId = uniqueId,
            Status = (int)EventStatus.Complete
        });

        await mongoDbContext.SaveChangesAsync(cancellationToken);

        await telegramService.SendMessageAsync("Membuat Cron Job");

        await mediator.Send(new RegisterRssJobUrlRequest
        {
            ChatId = request.ChatIdentifier,
            ThreadId = request.MessageThreadId,
            Url = rssUrl,
            JobId = uniqueId
        }, cancellationToken);

        return await telegramService.SendMessageAsync("RSS Berhasil disimpan");

    }
}