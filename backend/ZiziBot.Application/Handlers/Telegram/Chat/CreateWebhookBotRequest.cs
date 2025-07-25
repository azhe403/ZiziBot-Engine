using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZiziBot.Common.Types;
using ZiziBot.Database.MongoDb.Entities;

namespace ZiziBot.Application.Handlers.Telegram.Chat;

public class CreateWebhookBotRequest : BotRequestBase
{ }

public class CreateWebhookHandler(
    ILogger<CreateWebhookHandler> logger,
    DataFacade dataFacade,
    ServiceFacade serviceFacade
) : IBotRequestHandler<CreateWebhookBotRequest>
{
    public async Task<BotResponseBase> Handle(CreateWebhookBotRequest request, CancellationToken cancellationToken)
    {
        serviceFacade.TelegramService.SetupResponse(request);

        await serviceFacade.TelegramService.SendMessageText("Sedang membuat webhook..");

        var webhookChat = await dataFacade.MongoDb.WebhookChat
            .Where(entity => entity.ChatId == request.ChatIdentifier)
            .Where(entity => entity.Status == EventStatus.Complete)
            .Where(entity => entity.MessageThreadId == request.MessageThreadId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        var routeId = await StringUtil.GetNanoIdAsync();
        var webhookApi = EnvUtil.GetEnv(Env.TELEGRAM_WEBHOOK_URL) + "/api/webhook/";
        var webhookUrl = webhookApi + routeId;
        var htmlMessage = HtmlMessage.Empty
            .Bold("Webhook").Br();

        if (webhookChat == null)
        {
            dataFacade.MongoDb.WebhookChat.Add(new WebhookChatEntity() {
                ChatId = request.ChatIdentifier,
                MessageThreadId = request.MessageThreadId,
                RouteId = routeId,
                Status = EventStatus.Complete,
            });
        }
        else
        {
            webhookUrl = webhookApi + webhookChat.RouteId;
        }

        htmlMessage.Code(webhookUrl);
        await dataFacade.MongoDb.SaveChangesAsync(cancellationToken);

        return await serviceFacade.TelegramService.EditMessageText(htmlMessage.ToString());
    }
}