using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Chat;

public class CreateWebhookBotRequest : BotRequestBase
{
}

public class CreateWebhookHandler : IRequestHandler<CreateWebhookBotRequest, BotResponseBase>
{
    private readonly ILogger<CreateWebhookHandler> _logger;
    private readonly TelegramService _telegramService;
    private readonly MongoDbContextBase _mongoDbContext;

    public CreateWebhookHandler(ILogger<CreateWebhookHandler> logger, TelegramService telegramService, MongoDbContextBase mongoDbContext)
    {
        _logger = logger;
        _telegramService = telegramService;
        _mongoDbContext = mongoDbContext;
    }

    public async Task<BotResponseBase> Handle(CreateWebhookBotRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        await _telegramService.SendMessageText("Sedang membuat webhook..");

        var webhookChat = await _mongoDbContext.WebhookChat
            .Where(entity => entity.ChatId == request.ChatIdentifier)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .Where(entity => entity.MessageThreadId == request.MessageThreadId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        var routeId = await StringUtil.GetNanoIdAsync();
        var webhookApi = EnvUtil.GetEnv(Env.WEB_CONSOLE_URL) + "/api/webhook/";
        var webhookUrl = webhookApi + routeId;
        var htmlMessage = HtmlMessage.Empty
            .Bold("Webhook").Br();

        if (webhookChat == null)
        {
            _mongoDbContext.WebhookChat.Add(new WebhookChatEntity()
            {
                ChatId = request.ChatIdentifier,
                MessageThreadId = request.MessageThreadId,
                RouteId = routeId,
                Status = (int)EventStatus.Complete,
            });
        }
        else
        {
            webhookUrl = webhookApi + webhookChat.RouteId;
        }

        htmlMessage.Code(webhookUrl);
        await _mongoDbContext.SaveChangesAsync(cancellationToken);

        return await _telegramService.EditMessageText(htmlMessage.ToString());
    }
}