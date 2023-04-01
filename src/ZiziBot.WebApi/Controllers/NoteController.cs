using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NoteController : ApiControllerBase
{
    [HttpGet()]
    [AccessLevel(AccessLevelEnum.AdminOrPrivate)]
    public async Task<IActionResult> GetNotes([FromQuery] GetNoteRequest request)
    {
        return await SendRequest(request);
    }

    [HttpPost()]
    public async Task<IActionResult> CreateNote(CreateNoteRequest request)
    {
        return await SendRequest(request);
    }
}