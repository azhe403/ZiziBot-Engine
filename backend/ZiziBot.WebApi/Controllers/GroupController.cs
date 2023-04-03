using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupController : ApiControllerBase
{
    [HttpGet("welcome-message")]
    [AccessLevel(AccessLevelEnum.AdminOrPrivate)]
    public async Task<IActionResult> GetWelcomeMessage(GetWelcomeMessageRequest request)
    {
        return await SendRequest(request);
    }

    [HttpPost("welcome-message")]
    [AccessLevel(AccessLevelEnum.AdminOrPrivate)]
    public async Task<IActionResult> SaveWelcomeMessage(SaveWelcomeMessageRequest request)
    {
        return await SendRequest(request);
    }
}