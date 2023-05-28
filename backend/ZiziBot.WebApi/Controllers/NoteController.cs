using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NoteController : ApiControllerBase
{
    [HttpGet()]
    [AccessFilter(checkHeader: true)]
    public async Task<IActionResult> GetNotes([FromQuery] ListNoteRequest request)
    {
        return await SendRequest(request);
    }

    [HttpGet("{NoteId}")]
    [AccessFilter(checkHeader: true)]
    public async Task<IActionResult> GetNote(GetNoteRequest request)
    {
        return await SendRequest(request);
    }

    [HttpPost()]
    [AccessFilter(checkHeader: true)]
    public async Task<IActionResult> CreateNote(SaveNoteRequest request)
    {
        return await SendRequest(request);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteNote(DeleteNoteRequest request)
    {
        return await SendRequest(request);
    }
}