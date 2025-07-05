using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NoteController : ApiControllerBase
{
    [HttpGet()]
    [AccessFilter(flag: Flag.REST_CHAT_NOTE_LIST, roleLevel: RoleLevel.ChatAdmin)]
    public async Task<IActionResult> GetListNotes([FromQuery] ListNoteRequest request)
    {
        return await SendRequest(request);
    }

    [HttpGet("{NoteId}")]
    [AccessFilter(flag: Flag.REST_CHAT_NOTE_GET, roleLevel: RoleLevel.ChatAdmin)]
    public async Task<IActionResult> GetDetailNote(GetNoteRequest request)
    {
        return await SendRequest(request);
    }

    [HttpPost()]
    [AccessFilter(flag: Flag.REST_CHAT_NOTE_CREATE, roleLevel: RoleLevel.ChatAdmin)]
    public async Task<IActionResult> CreateNote(SaveNoteRequest request)
    {
        return await SendRequest(request);
    }

    [HttpDelete]
    [AccessFilter(flag: Flag.REST_CHAT_NOTE_DELETE, roleLevel: RoleLevel.ChatAdmin)]
    public async Task<IActionResult> DeleteNote(DeleteNoteRequest request)
    {
        return await SendRequest(request);
    }
}