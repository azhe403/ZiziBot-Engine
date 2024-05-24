using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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

    public WebhookSource WebhookSource => UserAgent.GetWebHookSource();
}

public class PostWebhookPayloadResponseDto
{
    public TimeSpan Duration { get; set; }
}

public class PostWebhookPayloadHandler(
    AppSettingRepository appSettingRepository,
    ChatSettingRepository chatSettingRepository,
    GithubWebhookEventProcessor githubWebhookEventProcessor)
    : IRequestHandler<PostWebhookPayloadRequest, ApiResponseBase<PostWebhookPayloadResponseDto>>
{
    public async Task<ApiResponseBase<PostWebhookPayloadResponseDto>> Handle(PostWebhookPayloadRequest request,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new ApiResponseBase<PostWebhookPayloadResponseDto>() {
            TransactionId = request.HttpContextAccessor?.HttpContext?.TraceIdentifier ?? string.Empty
        };

        if (request.Content == null)
        {
            return response.BadRequest("Webhook payload is empty");
        }

        var botSetting = await appSettingRepository.GetBotMain();

        var webhookChat = await chatSettingRepository.GetWebhookRouteById(request.targetId);

        if (webhookChat == null)
        {
            return response.BadRequest("Webhook route not found");
        }

        if (request.IsDebug)
        {
        }

        switch (request.WebhookSource)
        {
            case WebhookSource.GitHub:
                githubWebhookEventProcessor.RouteId = webhookChat.RouteId;
                githubWebhookEventProcessor.ChatId = webhookChat.ChatId;
                githubWebhookEventProcessor.ThreadId = webhookChat.MessageThreadId;
                githubWebhookEventProcessor.Token = botSetting.Token;

                await githubWebhookEventProcessor.ProcessWebhookAsync(request.Headers, request.Content.ToString());
                break;
            case WebhookSource.GitLab:
            case WebhookSource.Unknown:
            default:
                response.BadRequest("Webhook source is unknown");
                break;
        }

        response.Result = new PostWebhookPayloadResponseDto() {
            Duration = stopwatch.Elapsed
        };

        stopwatch.Stop();

        return response.Success("Webhook payload processed");
    }
}