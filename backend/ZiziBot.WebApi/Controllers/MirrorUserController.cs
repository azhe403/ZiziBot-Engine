using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/mirror-user")]
public class MirrorUserController : ApiControllerBase
{
    [HttpGet()]
    [Authorize(Roles = "Sudoer")]
    public async Task<IActionResult> GetUsersAll(GetMirrorUsersRequestDto request)
    {
        return await SendRequest(request);
    }

    [HttpGet("find")]
    public async Task<IActionResult> GetUserByUserId([FromQuery] GetMirrorUserByUserIdRequestDto request)
    {
        return await SendRequest(request);
    }

    [HttpPost()]
    public async Task<IActionResult> PostUserMirror([FromBody] PostMirrorUserRequestDto request)
    {
        return await SendRequest(request);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteMirrorUser([FromQuery] DeleteMirrorUserRequestDto request)
    {
        return await SendRequest(request);
    }
}