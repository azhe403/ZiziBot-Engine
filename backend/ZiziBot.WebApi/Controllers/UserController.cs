using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ApiControllerBase
{
    [HttpGet]
    [AccessFilter(checkHeader: true)]
    public IActionResult Index()
    {
        return Ok(true);
    }

    [HttpPost("session/telegram")]
    [AccessFilter(checkHeader: true)]
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    public async Task<IActionResult> PostTelegramSession(SaveTelegramSessionRequest request)
    {
        return await SendRequest(request);
    }

    [HttpPost("session/telegram/validate")]
    [AccessFilter(checkHeader: true)]
    [AllowAnonymous]
    public async Task<IActionResult> CheckDashboardSession([FromBody] CheckDashboardSessionRequestDto request)
    {
        return await SendRequest(request);
    }

    [HttpPost("session/validate")]
    [AccessFilter(checkHeader: true)]
    [AllowAnonymous]
    public async Task<IActionResult> CheckDashboardSessionId(CheckDashboardBearerSessionRequestDto request)
    {
        return await SendRequest(request);
    }

    [HttpGet("list-group")]
    [AccessFilter(checkHeader: true)]
    [EnableRateLimiting(RateLimitingPolicy.API_LIST_RATE_LIMITING_KEY)]
    public async Task<IActionResult> GetListGroup([FromQuery] GetListGroupRequest request)
    {
        return await SendRequest(request);
    }
}