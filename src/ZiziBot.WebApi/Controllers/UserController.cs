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
	public async Task<IActionResult> PostTelegramSession([FromBody] SaveTelegramSessionRequestModel requestModel)
	{
		var result = await Mediator.Send(requestModel);
		return Ok(result);
	}
}