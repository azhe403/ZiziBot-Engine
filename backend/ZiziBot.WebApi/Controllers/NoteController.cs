using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NoteController : ApiControllerBase
{
    [HttpGet()]
    public async Task<IActionResult> GetNotes([FromQuery] ListNoteRequest request)
    {
        return await SendRequest(request);
    }

    [HttpPost()]
    public async Task<IActionResult> CreateNote(CreateNoteRequest request)
    {
        return await SendRequest(request);
    }
}