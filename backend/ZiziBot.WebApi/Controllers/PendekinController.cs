using Microsoft.AspNetCore.Mvc;
using ZiziBot.Application.Handlers.RestApis.Pendekin;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PendekinController : ApiControllerBase
{
    [HttpPost]
    [AccessFilter(flag: Flag.REST_PRODUCTIVITY_PENDEKIN_CREATE)]
    public async Task<IActionResult> CreatePendekin(CreatePendekinRequest request)
    {
        return await SendRequest(request);
    }

    [HttpGet("{ShortPath}")]
    [AccessFilter(flag: Flag.REST_PRODUCTIVITY_PENDEKIN_GET)]
    public async Task<IActionResult> GetPendekin(GetPendekinRequest request)
    {
        return await SendRequest(request);
    }

    [HttpGet()]
    [AccessFilter(flag: Flag.REST_PRODUCTIVITY_PENDEKIN_LIST)]
    public async Task<IActionResult> ListPendekin(ListPendekinRequest request)
    {
        return await SendRequest(request);
    }
}