using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupController : ApiControllerBase
{
    [HttpGet("welcome-message")]
    [AccessFilter(checkHeader: true)]
    public async Task<IActionResult> ListWelcomeMessage(ListWelcomeMessageRequest request)
    {
        return await SendRequest(request);
    }

    [HttpGet("welcome-message/{WelcomeId}")]
    [AccessFilter(checkHeader: true)]
    public async Task<IActionResult> GetWelcomeMessage(GetWelcomeMessageRequest request)
    {
        return await SendRequest(request);
    }

    [HttpPost("welcome-message")]
    [AccessFilter(checkHeader: true)]
    public async Task<IActionResult> SaveWelcomeMessage(SaveWelcomeMessageRequest request)
    {
        return await SendRequest(request);
    }

    [HttpDelete("welcome-message")]
    [AccessFilter(checkHeader: true)]
    public async Task<IActionResult> DeleteWelcomeMessage(DeleteWelcomeMessageRequest request)
    {
        return await SendRequest(request);
    }

    [HttpPost("select-welcome-message")]
    [AccessFilter(checkHeader: true)]
    public async Task<IActionResult> SelectWelcomeMessage(SelectWelcomeMessageRequest request)
    {
        return await SendRequest(request);
    }
}