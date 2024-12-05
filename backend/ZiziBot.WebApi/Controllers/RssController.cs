using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RssController : ApiControllerBase
{
    [HttpGet()]
    [AccessFilter(flag: Flag.REST_CHAT_RSS_LIST)]
    public async Task<IActionResult> Index(GetListRssRequest request)
    {
        return await SendRequest(request);
    }

    [HttpPost]
    [AccessFilter(flag: Flag.REST_CHAT_RSS_CREATE)]
    public async Task<IActionResult> Save(SaveRssRequest request)
    {
        return await SendRequest(request);
    }
}