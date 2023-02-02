using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AntiSpamController : ApiControllerBase
{
    [HttpPost("ess-ban")]
    public async Task<IActionResult> PostEs2BanListAsync([FromBody] PostGlobalBanApiRequest request)
    {
        var result = await Mediator.Send(request);
        return SwitchStatus(result);
    }

    [HttpDelete("ess-ban")]
    public async Task<IActionResult> DeleteEs2BanListAsync([FromBody] DeleteGlobalBanApiRequest request)
    {
        var result = await Mediator.Send(request);
        return SwitchStatus(result);
    }

    [HttpGet("ess-ban")]
    public async Task<IActionResult> GetEs2BanListAsync()
    {
        var result = await Mediator.Send(new GetGlobalBanApiRequest());
        return SwitchStatus(result);
    }

    [HttpGet("check-ban")]
    public async Task<IActionResult> GetEs2BanByUserIdAsync()
    {
        var result = await Mediator.Send(new GetGlobalBanApiRequest());
        return SwitchStatus(result);
    }

    [HttpPut("undelete-ban")]
    public async Task<IActionResult> UndeleteEs2BanByUserIdAsync([FromBody] UndeleteGlobalBanApiRequest request)
    {
        var result = await Mediator.Send(request);
        return SwitchStatus(result);
    }
}