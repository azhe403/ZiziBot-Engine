using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.Application.Handlers.Telegram.Chat;

public class GetSettingPanelBotRequestModel : BotRequestBase
{ }

public class GetSettingPanelRequestHandler(
    ILogger<GetSettingPanelRequestHandler> logger,
    DataFacade dataFacade,
    ServiceFacade serviceFacade
)
    : IBotRequestHandler<GetSettingPanelBotRequestModel>
{
    public async Task<BotResponseBase> Handle(
        GetSettingPanelBotRequestModel request,
        CancellationToken cancellationToken
    )
    {
        serviceFacade.TelegramService.SetupResponse(request);

        var chat = await dataFacade.MongoEf.ChatSetting
            .Where(x => x.Status == EventStatus.Complete)
            .Where(x => x.ChatId == request.ChatIdentifier)
            .FirstOrDefaultAsync(cancellationToken);

        if (chat == null)
        {
            dataFacade.MongoEf.ChatSetting.Add(new ChatSettingEntity {
                ChatId = request.ChatIdentifier,
                Status = EventStatus.Complete,
            });

            await dataFacade.MongoEf.SaveChangesAsync(cancellationToken);
        }

        return await serviceFacade.TelegramService.SendMessageText("Sedang memuat tombol..");
    }
}