using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Chat;

public class CreateWebhookRequest : RequestBase
{
}

public class CreateWebhookHandler : IRequestHandler<CreateWebhookRequest, ResponseBase>
{
    private readonly ILogger<CreateWebhookHandler> _logger;
    private readonly TelegramService _telegramService;
    private readonly ChatDbContext _chatDbContext;

    public CreateWebhookHandler(ILogger<CreateWebhookHandler> logger, TelegramService telegramService, ChatDbContext chatDbContext)
    {
        _logger = logger;
        _telegramService = telegramService;
        _chatDbContext = chatDbContext;
    }

    public async Task<ResponseBase> Handle(CreateWebhookRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        await _telegramService.SendMessageText("Sedang membuat webhook..");

        var webhookChat = await _chatDbContext.WebhookChat
            .FirstOrDefaultAsync(entity =>
                    entity.ChatId == request.ChatIdentifier &&
                    entity.Status == (int)EventStatus.Complete,
                cancellationToken: cancellationToken);


        var routeId = await StringUtil.GetNanoIdAsync();
        var webhookUrl = UrlConst.WEBHOOK_URL + routeId;
        var htmlMessage = HtmlMessage.Empty
            .Bold("Webhook").Br();

        if (webhookChat == null)
        {
            _chatDbContext.WebhookChat.Add(new WebhookChatEntity()
            {
                ChatId = request.ChatIdentifier,
                RouteId = routeId,
                Status = (int)EventStatus.Complete,
            });
        }
        else
        {
            webhookUrl = UrlConst.WEBHOOK_URL + webhookChat.RouteId;
        }

        htmlMessage.Code(webhookUrl);
        await _chatDbContext.SaveChangesAsync(cancellationToken);

        return await _telegramService.EditMessageText(htmlMessage.ToString());
    }
}