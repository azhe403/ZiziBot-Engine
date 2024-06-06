using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using ZiziBot.DataSource.MongoDb.Entities;

namespace ZiziBot.Application.Handlers.RestApis.Webhook;

public class PostWebhookPayloadRequest : ApiRequestBase<PostWebhookPayloadResponseDto>
{
    [FromBody]
    public object? Content { get; set; }

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
    ChatSettingRepository chatSettingRepository,
    GithubWebhookEventProcessor githubWebhookEventProcessor)
    : IRequestHandler<PostWebhookPayloadRequest, ApiResponseBase<PostWebhookPayloadResponseDto>>
{
    public async Task<ApiResponseBase<PostWebhookPayloadResponseDto>> Handle(
        PostWebhookPayloadRequest request,
        CancellationToken cancellationToken
    )
    {
        var stopwatch = Stopwatch.StartNew();
        var webhookSource = request.UserAgent.GetWebHookSource();
        var webhookHeader = WebhookHeader.Parse(request.Headers);
        var content = $"{request.Content}";
        var response = new ApiResponseBase<PostWebhookPayloadResponseDto>() {
            TransactionId = request.HttpContextAccessor?.HttpContext?.TraceIdentifier ?? string.Empty
        };

        if (request.Content == null)
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

        switch (webhookSource)
        {
            case WebhookSource.GitHub:
                githubWebhookEventProcessor.RouteId = webhookChat.RouteId;
                githubWebhookEventProcessor.Payload = request.Content.ToString();
                githubWebhookEventProcessor.TransactionId = $"{request.TransactionId}";

                await githubWebhookEventProcessor.ProcessWebhookAsync(request.Headers, request.Content.ToString());
                break;
            case WebhookSource.Unknown:
            default:
                response.BadRequest("Webhook source is unknown");
                break;
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

        try
        {
            sentMessage = await botClient.SendTextMessageAsync(
                chatId: webhookChat.ChatId,
                text: webhookResponse.FormattedHtml,
                messageThreadId: webhookChat.MessageThreadId,
                parseMode: ParseMode.Html,
                disableWebPagePreview: true, cancellationToken: cancellationToken);
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
            TransactionId = $"{request.TransactionId}",
            ChatId = webhookChat.ChatId,
            MessageId = sentMessage.MessageId,
            WebhookSource = WebhookSource.GitHub,
            Elapsed = stopwatch.Elapsed,
            Payload = request.IsDebug ? content : string.Empty,
            Header = request.IsDebug ? webhookHeader : default,
            Status = (int)EventStatus.Complete
        });

        await mongoDbContextBase.SaveChangesAsync(cancellationToken);

        mongoDbContextBase.ChatActivity.Add(new ChatActivityEntity {
            ActivityType = ChatActivityType.BotSentWebHook,
            ChatId = webhookChat.ChatId,
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