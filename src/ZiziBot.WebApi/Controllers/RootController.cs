using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api")]
public class RootController : ApiControllerBase
{
	[HttpGet]
	public IActionResult Get()
	{
		return Ok("Welcome to Zizi 5 API");
	}
}