using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Chat;

public class ConnectChannelPostRequest : BotRequestBase
{
    public long ChannelId { get; set; }
}

public class ConnectChannelPostHandler : IBotRequestHandler<ConnectChannelPostRequest>
{
    private readonly ILogger<ConnectChannelPostHandler> _logger;
    private readonly TelegramService _telegramService;
    private readonly MongoDbContextBase _mongoDbContext;

    public ConnectChannelPostHandler(ILogger<ConnectChannelPostHandler> logger, TelegramService telegramService, MongoDbContextBase mongoDbContext)
    {
        _logger = logger;
        _telegramService = telegramService;
        _mongoDbContext = mongoDbContext;
    }

    public async Task<BotResponseBase> Handle(ConnectChannelPostRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        await _telegramService.SendMessageText("Sedang menautkan Kanal..");

        if (request.ChannelId == 0)
        {
            return await _telegramService.EditMessageText("Spesifikasikan ChannelId yang ingin ditautkan");
        }

        var channelMap = await _mongoDbContext.ChannelMap.AsNoTracking()
            .Where(entity => entity.ChannelId == request.ChannelId)
            .Where(entity => entity.ChatId == request.ChatIdentifier)
            .Where(entity => entity.ThreadId == request.MessageThreadId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (channelMap == null)
        {
            _mongoDbContext.ChannelMap.Add(new ChannelMapEntity()
            {
                ChannelId = request.ChannelId,
                ThreadId = request.MessageThreadId,
                ChatId = request.ChatIdentifier,
                Status = (int)EventStatus.Complete
            });

            await _mongoDbContext.SaveChangesAsync(cancellationToken);
        }

        await _telegramService.EditMessageText("Berhasil menautkan Kanal.");

        return _telegramService.Complete();
    }
}