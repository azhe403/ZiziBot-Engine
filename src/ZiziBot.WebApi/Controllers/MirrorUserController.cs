using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/mirror-user")]
public class MirrorUserController : ApiControllerBase
{
    [HttpGet()]
    [Authorize(Roles = "Sudoer")]
    public async Task<IActionResult> GetUsersAll()
    {
        var mirrorUsers = await Mediator.Send(new GetMirrorUsersRequestDto());
        return SwitchStatus(mirrorUsers);
    }

    [HttpGet("find")]
    public async Task<IActionResult> GetUserByUserId([FromQuery] GetMirrorUserByUserIdRequestDto requestDto)
    {
        var mirrorUser = await Mediator.Send(requestDto);
        return SwitchStatus(mirrorUser);
    }

    [HttpPost()]
    public async Task<IActionResult> PostUserMirror([FromBody] PostMirrorUserRequestDto requestDto)
    {
        var result = await Mediator.Send(requestDto);
        return SwitchStatus(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteMirrorUser([FromQuery] DeleteMirrorUserRequestDto requestDto)
    {
        var result = await Mediator.Send(requestDto);
        return Ok(result);
    }
}