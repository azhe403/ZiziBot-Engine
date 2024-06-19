using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using ZiziBot.DataSource.MongoDb.Entities;

namespace ZiziBot.Application.Handlers.RestApis.Webhook;

public class PostWebhookPayloadRequest : ApiRequestBase<PostWebhookPayloadResponseDto>
{
    [FromRoute(Name = "targetId")]
    public string targetId { get; set; }

    [FromHeader(Name = "User-Agent")]
    public string UserAgent { get; set; }

    [FromQuery(Name = "webhookFormat")]
    public WebhookFormat WebhookFormat { get; set; }

    [FromQuery(Name = "isDebug")]
    public bool IsDebug { get; set; }
}

public class PostWebhookPayloadResponseDto
{
    public TimeSpan Duration { get; set; }
    public object Result { get; set; }
}

public class PostWebhookPayloadHandler(
    ILogger<PostWebhookPayloadHandler> logger,
    IMediator mediator,
    MediatorService mediatorService,
    MongoDbContextBase mongoDbContextBase,
    WebhookService webhookService,
    AppSettingRepository appSettingRepository,
    ChatSettingRepository chatSettingRepository
) : IRequestHandler<PostWebhookPayloadRequest, ApiResponseBase<PostWebhookPayloadResponseDto>>
{
    public async Task<ApiResponseBase<PostWebhookPayloadResponseDto>> Handle(
        PostWebhookPayloadRequest request,
        CancellationToken cancellationToken
    )
    {
        var stopwatch = Stopwatch.StartNew();
        var webhookSource = request.UserAgent.GetWebHookSource();
        var webhookHeader = WebhookHeader.Parse(request.Headers);
        var content = await request.RequestBody();
        var response = new ApiResponseBase<PostWebhookPayloadResponseDto>() {
            TransactionId = request.HttpContextAccessor?.HttpContext?.TraceIdentifier ?? string.Empty
        };

        if (content == null)
        {
            return response.BadRequest("Webhook payload is empty");
        }

        var webhookChat = await chatSettingRepository.GetWebhookRouteById(request.targetId);

        if (webhookChat == null)
        {
            return response.BadRequest("Webhook route not found");
        }

        if (request.IsDebug)
        {
        }

        var webhookResponse = webhookSource switch {
            WebhookSource.GitHub => await webhookService.ParseGitHub(webhookHeader, content),
            WebhookSource.GitLab => await webhookService.ParseGitLab(webhookHeader, content),
            _ => default
        };

        if (webhookResponse == null)
        {
            return response.BadRequest("Webhook can't be processed");
        }

        var botSetting = await appSettingRepository.GetBotMain();
        var botClient = new TelegramBotClient(botSetting.Token);

        Message sentMessage = new();

        var lastMessageId = await chatSettingRepository.LastWebhookMessageBetterEdit(webhookChat.ChatId, webhookSource, webhookHeader.Event);

        try
        {
            if (lastMessageId != 0)
            {
                sentMessage = await botClient.EditMessageTextAsync(
                    chatId: webhookChat.ChatId,
                    messageId: lastMessageId,
                    text: webhookResponse.FormattedHtml,
                    parseMode: ParseMode.Html,
                    disableWebPagePreview: true, cancellationToken: cancellationToken);
            }
            else
            {
                sentMessage = await botClient.SendTextMessageAsync(
                    chatId: webhookChat.ChatId,
                    text: webhookResponse.FormattedHtml,
                    messageThreadId: webhookChat.MessageThreadId,
                    parseMode: ParseMode.Html,
                    disableWebPagePreview: true, cancellationToken: cancellationToken);
            }
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Trying send GitHub Webhook without thread to ChatId: {ChatId}", webhookChat.ChatId);
            if (exception.Message.Contains("thread not found"))
            {
                sentMessage = await botClient.SendTextMessageAsync(
                    chatId: webhookChat.ChatId,
                    text: webhookResponse.FormattedHtml,
                    parseMode: ParseMode.Html,
                    disableWebPagePreview: true, cancellationToken: cancellationToken);
            }
        }

        mongoDbContextBase.WebhookHistory.Add(new WebhookHistoryEntity {
            RouteId = webhookChat.RouteId,
            TransactionId = request.TransactionId,
            CreatedDate = default,
            UpdatedDate = default,
            ChatId = webhookChat.ChatId,
            MessageId = sentMessage.MessageId,
            MessageThreadId = 0,
            WebhookSource = WebhookSource.GitHub,
            Elapsed = stopwatch.Elapsed,
            Payload = request.IsDebug ? content : string.Empty,
            Header = request.IsDebug ? request.Headers.ToHeaderRawKv() : default,
            EventName = webhookHeader.Event,
            Status = (int)EventStatus.Complete
        });

        await mongoDbContextBase.SaveChangesAsync(cancellationToken);

        mongoDbContextBase.ChatActivity.Add(new ChatActivityEntity {
            ActivityType = lastMessageId == 0 ? ChatActivityType.BotSendWebHook : ChatActivityType.BotEditWebHook,
            ChatId = webhookChat.ChatId,
            UserId = sentMessage.From.Id,
            Chat = sentMessage.Chat,
            User = sentMessage.From,
            Status = (int)EventStatus.Complete,
            TransactionId = request.TransactionId,
            MessageId = sentMessage.MessageId
        });

        await mongoDbContextBase.SaveChangesAsync(cancellationToken);

        stopwatch.Stop();

        return response.Success("Webhook payload processed", new PostWebhookPayloadResponseDto() {
            Duration = stopwatch.Elapsed
        });
    }
}