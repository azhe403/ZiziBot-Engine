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

    [HttpGet("{NoteId}")]
    public async Task<IActionResult> GetNote(GetNoteRequest request)
    {
        return await SendRequest(request);
    }

    [HttpPost()]
    public async Task<IActionResult> CreateNote(SaveNoteRequest request)
    {
        return await SendRequest(request);
    }
}