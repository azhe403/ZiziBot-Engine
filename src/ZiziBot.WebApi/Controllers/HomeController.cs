using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Welcome to Zizi 5 API");
    }
}
