using Microsoft.Extensions.Logging;
using MongoFramework.Linq;
using ZiziBot.DataSource.MongoDb.Entities;

namespace ZiziBot.Application.Handlers.Telegram.Chat;

public class GetSettingPanelBotRequestModel : BotRequestBase
{
}

public class GetSettingPanelRequestHandler(
    ILogger<GetSettingPanelRequestHandler> logger,
    TelegramService telegramService,
    MongoDbContextBase mongoDbContext)
    : IRequestHandler<GetSettingPanelBotRequestModel, BotResponseBase>
{
    private readonly ILogger<GetSettingPanelRequestHandler> _logger = logger;

    public async Task<BotResponseBase> Handle(GetSettingPanelBotRequestModel request,
        CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        var chat = await mongoDbContext.ChatSetting.FirstOrDefaultAsync(x => x.ChatId == request.ChatIdentifier,
            cancellationToken);
        if (chat == null)
        {
            mongoDbContext.ChatSetting.Add(new ChatSettingEntity() {
                ChatId = request.ChatIdentifier,
            });

            await mongoDbContext.SaveChangesAsync(cancellationToken);
        }

        await telegramService.SendMessageText("Sedang memuat tombol..");

        return telegramService.Complete();
    }
}