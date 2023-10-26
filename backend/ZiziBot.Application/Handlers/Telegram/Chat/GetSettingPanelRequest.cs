using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Chat;

public class GetSettingPanelBotRequestModel : BotRequestBase
{

}

public class GetSettingPanelRequestHandler : IRequestHandler<GetSettingPanelBotRequestModel, BotResponseBase>
{
    private readonly ILogger<GetSettingPanelRequestHandler> _logger;
    private readonly TelegramService _telegramService;
    private readonly MongoDbContextBase _mongoDbContext;

    public GetSettingPanelRequestHandler(ILogger<GetSettingPanelRequestHandler> logger, TelegramService telegramService, MongoDbContextBase mongoDbContext)
    {
        _logger = logger;
        _telegramService = telegramService;
        _mongoDbContext = mongoDbContext;
    }

    public async Task<BotResponseBase> Handle(GetSettingPanelBotRequestModel request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        var chat = await _mongoDbContext.ChatSetting.FirstOrDefaultAsync(x => x.ChatId == request.ChatIdentifier, cancellationToken);
        if (chat == null)
        {
            _mongoDbContext.ChatSetting.Add(new ChatSettingEntity()
            {
                ChatId = request.ChatIdentifier,
            });

            await _mongoDbContext.SaveChangesAsync(cancellationToken);
        }

        await _telegramService.SendMessageText("Sedang memuat tombol..");

        return _telegramService.Complete();
    }
}