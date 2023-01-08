using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("mirror-user")]
public class MirrorUserController : ApiControllerBase
{
	[HttpGet()]
	public async Task<IActionResult> GetUsersAll()
	{
		var mirrorUsers = await Mediator.Send(new GetMirrorUsersRequestDto());
		return Ok(mirrorUsers);
	}

	[HttpGet("find")]
	public async Task<IActionResult> GetUsers([FromQuery] GetMirrorUserByUserIdRequestDto requestDto)
	{
		var mirrorUser = await Mediator.Send(requestDto);
		return Ok(mirrorUser);
	}

	[HttpPost()]
	public async Task<bool> PostUserMirror([FromBody] PostMirrorUserRequestDto requestDto)
	{
		var result = await Mediator.Send(requestDto);

		return result;
	}
}