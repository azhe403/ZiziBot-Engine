using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    protected IMediator Mediator => HttpContext.RequestServices.GetRequiredService<IMediator>();
    
    [HttpGet]
    public IActionResult Index()
    {
        return Ok(true);
    }

    [HttpGet("session/telegram")]
    public async Task<IActionResult> GetTelegramSession([FromQuery] SaveTelegramSessionRequestModel requestModel)
    {
        var result = await Mediator.Send(requestModel);
        return Ok(result);
    }

    [HttpPost("session/telegram")]
    public IActionResult PostTelegramSession()
    {
        return Ok(true);
    }
}