using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PendekinController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreatePendekin(CreatePendekinRequest request)
    {
        return await SendRequest(request);
    }

    [HttpGet("{ShortPath}")]
    public async Task<IActionResult> GetPendekin(GetPendekinRequest request)
    {
        return await SendRequest(request);
    }

    [HttpGet()]
    public async Task<IActionResult> ListPendekin(ListPendekinRequest request)
    {
        return await SendRequest(request);
    }
}