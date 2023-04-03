using System.Net;
using Microsoft.AspNetCore.Mvc;
using MongoFramework.Linq;
using Telegram.Bot;

namespace ZiziBot.Application.Handlers.RestApis.Webhook;

public class PostWebhookPayloadRequest : ApiRequestBase<PostWebhookPayloadResponseDto>
{
    [FromBody]
    public object Content { get; set; }

    [FromRoute(Name = "targetId")]
    public string targetId { get; set; }

    [FromHeader(Name = "User-Agent")]
    public string UserAgent { get; set; }

    public WebhookSource WebhookSource => UserAgent.GetWebHookSource();
}

public class PostWebhookPayloadResponseDto
{
}

public class PostWebhookPayloadHandler : IRequestHandler<PostWebhookPayloadRequest, ApiResponseBase<PostWebhookPayloadResponseDto>>
{
    private readonly AppSettingsDbContext _appSettingsDbContext;
    private readonly ChatDbContext _chatDbContext;

    public PostWebhookPayloadHandler(AppSettingsDbContext appSettingsDbContext, ChatDbContext chatDbContext)
    {
        _appSettingsDbContext = appSettingsDbContext;
        _chatDbContext = chatDbContext;
    }

    public async Task<ApiResponseBase<PostWebhookPayloadResponseDto>> Handle(PostWebhookPayloadRequest request, CancellationToken cancellationToken)
    {
        var botSetting = await _appSettingsDbContext.BotSettings
            .FirstOrDefaultAsync(settings => settings.Name == "Main", cancellationToken: cancellationToken);

        var webhookChat = await _chatDbContext.WebhookChat
            .FirstOrDefaultAsync(entity =>
                    entity.RouteId == request.targetId &&
                    entity.Status == (int)EventStatus.Complete,
                cancellationToken: cancellationToken);

        if (webhookChat == null)
        {
            return new ApiResponseBase<PostWebhookPayloadResponseDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Webhook route not found",
            };
        }

        var botClient = new TelegramBotClient(botSetting.Token);
        await botClient.SendTextMessageAsync(webhookChat.ChatId, "Ini mesej webhuk", cancellationToken: cancellationToken);

        return new ApiResponseBase<PostWebhookPayloadResponseDto>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Webhook sent",
        };
    }
}