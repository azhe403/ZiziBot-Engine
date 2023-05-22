using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RssController : ApiControllerBase
{
    [HttpGet()]
    [AccessFilter(checkHeader: true)]
    public async Task<IActionResult> Index(GetListRssRequest request)
    {
        return await SendRequest(request);
    }

    [HttpPost]
    [AccessFilter(checkHeader: true)]
    public async Task<IActionResult> Save(SaveRssRequest request)
    {
        return await SendRequest(request);
    }
}