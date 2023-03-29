using Microsoft.AspNetCore.Mvc;
using ZiziBot.Application.Handlers.RestApis.Note;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NoteController : ApiControllerBase
{
    [HttpGet("all")]
    public async Task<IActionResult> GetAll([FromQuery] GetNoteRequestModel request)
    {
        return await SendRequest(request);
    }

    [HttpPost()]
    public async Task<IActionResult> CreateNote([FromBody] CreateNoteRequestModel request)
    {
        return await SendRequest(request);
    }
}