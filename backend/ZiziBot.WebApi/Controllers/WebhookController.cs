using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ApiControllerBase
{
    [HttpPost("{targetId}")]
    [AccessFilter(flag: Flag.REST_PRODUCTIVITY_WEBHOOK, checkHeader: false)]
    public async Task<IActionResult> ProcessingPayload(PostWebhookPayloadRequest request)
    {
        return await SendRequest(request);
    }
}