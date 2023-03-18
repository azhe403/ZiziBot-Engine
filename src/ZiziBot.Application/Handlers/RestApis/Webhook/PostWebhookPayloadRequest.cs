using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.Application.Handlers.RestApis.Webhook;

public class PostWebhookPayloadRequest : ApiRequestBase<bool>
{
    [FromBody]
    public object Content { get; set; }

    [FromRoute(Name = "targetId")]
    public string targetId { get; set; }
}

public class PostWebhookPayloadHandler : IRequestHandler<PostWebhookPayloadRequest, ApiResponseBase<bool>>
{
    public async Task<ApiResponseBase<bool>> Handle(PostWebhookPayloadRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}