using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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
    public TimeSpan Duration { get; set; }
}

public class PostWebhookPayloadHandler : IRequestHandler<PostWebhookPayloadRequest, ApiResponseBase<PostWebhookPayloadResponseDto>>
{
    private readonly AppSettingRepository _appSettingRepository;
    private readonly ChatSettingRepository _chatSettingRepository;
    private readonly GithubWebhookEventProcessor _githubWebhookEventProcessor;

    public PostWebhookPayloadHandler(AppSettingRepository appSettingRepository, ChatSettingRepository chatSettingRepository, GithubWebhookEventProcessor githubWebhookEventProcessor)
    {
        _appSettingRepository = appSettingRepository;
        _chatSettingRepository = chatSettingRepository;
        _githubWebhookEventProcessor = githubWebhookEventProcessor;
    }

    public async Task<ApiResponseBase<PostWebhookPayloadResponseDto>> Handle(PostWebhookPayloadRequest request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new ApiResponseBase<PostWebhookPayloadResponseDto>()
        {
            transactionId = request.HttpContextAccessor?.HttpContext?.TraceIdentifier ?? string.Empty
        };

        if (request.Content.ToString() == null)
        {
            return response.BadRequest("Webhook payload is empty");
        }

        var botSetting = await _appSettingRepository.GetBotMain();

        var webhookChat = await _chatSettingRepository.GetWebhookRouteById(request.targetId);

        if (webhookChat == null)
        {
            return response.BadRequest("Webhook route not found");
        }

        switch (request.WebhookSource)
        {
            case WebhookSource.GitHub:
                _githubWebhookEventProcessor.RouteId = webhookChat.RouteId;
                _githubWebhookEventProcessor.ChatId = webhookChat.ChatId;
                _githubWebhookEventProcessor.ThreadId = webhookChat.MessageThreadId;
                _githubWebhookEventProcessor.Token = botSetting.Token;

                await _githubWebhookEventProcessor.ProcessWebhookAsync(request.Headers, request.Content.ToString() ?? string.Empty);
                break;
            default:
                response.BadRequest("Webhook source is unknown");
                break;
        }

        response.Result = new PostWebhookPayloadResponseDto()
        {
            Duration = stopwatch.Elapsed
        };

        stopwatch.Stop();

        return response.Success("Webhook payload processed");
    }
}