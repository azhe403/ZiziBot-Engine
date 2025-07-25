using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZiziBot.Common.Types;

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
    IHttpContextHelper httpContextHelper,
    DataFacade dataFacade,
    ServiceFacade serviceFacade
) : IApiRequestHandler<PostWebhookPayloadRequest, PostWebhookPayloadResponseDto>
{
    public async Task<ApiResponseBase<PostWebhookPayloadResponseDto>> Handle(
        PostWebhookPayloadRequest request,
        CancellationToken cancellationToken
    )
    {
        logger.LogDebug("Receiving webhook with RouteId: {RouteId}", request.targetId);

        var stopwatch = Stopwatch.StartNew();
        var webhookSource = request.UserAgent.GetWebHookSource();
        var webhookHeader = WebhookHeader.Parse(httpContextHelper.HeaderDict);
        var content = await httpContextHelper.GetRawBodyAsync();
        var response = new ApiResponseBase<PostWebhookPayloadResponseDto>() {
            TransactionId = httpContextHelper.TransactionId
        };

        var webhookChat = await dataFacade.ChatSetting.GetWebhookRouteById(request.targetId);

        if (webhookChat == null)
        {
            return response.BadRequest("Webhook route not found");
        }

        logger.LogDebug("Processing Webhook {WebhookSource} to ChatId: {ChatId}", webhookSource, webhookChat.ChatId);

        var webhookResponse = webhookSource switch {
            WebhookSource.GitHub => await serviceFacade.WebhookService.ParseGitHub(webhookHeader, content),
            WebhookSource.GitLab => await serviceFacade.WebhookService.ParseGitLab(webhookHeader, content),
            _ => null
        };

        if (webhookResponse == null)
        {
            return response.BadRequest("Unsupported webhook source. UserAgent: " + request.UserAgent);
        }

        await serviceFacade.MediatorService.EnqueueAsync(new SendWebhookMessageRequest() {
            TargetId = request.targetId,
            Event = webhookHeader.Event,
            TransactionId = httpContextHelper.TransactionId,
            WebhookSource = webhookSource,
            RawHeaders = httpContextHelper.HeaderDict.ToHeaderRawKv(),
            RawBody = content,
            FormattedHtml = webhookResponse.FormattedHtml
        }, ExecutionStrategy.Hangfire);

        return response.Success("Webhook payload processed", new PostWebhookPayloadResponseDto() {
            Duration = stopwatch.Elapsed
        });
    }
}