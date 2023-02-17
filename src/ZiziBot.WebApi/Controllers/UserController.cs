using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ApiControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok(true);
    }

    [HttpPost("session/telegram")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    public async Task<IActionResult> PostTelegramSession([FromBody] SaveTelegramSessionRequestModel requestModel)
    {
        var result = await Mediator.Send(requestModel);
        return Ok(result);
    }

    [HttpPost("session/telegram/validate")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckDashboardSession([FromBody] CheckDashboardSessionRequestDto requestDto)
    {
        var result = await Mediator.Send(requestDto);
        return Ok(result);
    }

    [HttpPost("session/validate")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckDashboardSessionId([FromBody] CheckDashboardSessionIdRequestDto requestDto)
    {
        var result = await Mediator.Send(requestDto);
        return SwitchStatus(result);

    }
}