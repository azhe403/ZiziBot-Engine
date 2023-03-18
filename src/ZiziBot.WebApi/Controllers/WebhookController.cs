using Microsoft.AspNetCore.Mvc;
using ZiziBot.Application.Handlers.RestApis.Webhook;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ApiControllerBase
{
    [HttpPost("{targetId}")]
    public async Task<IActionResult> ProcessingPayload(PostWebhookPayloadRequest request)
    {
        var result = await Mediator.Send(request);
        return SwitchStatus(result);
    }
}