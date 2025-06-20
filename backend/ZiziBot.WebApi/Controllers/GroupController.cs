using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupController : ApiControllerBase
{
    [HttpGet("welcome-message")]
    [AccessFilter(flag: Flag.REST_GROUP_WELCOME_MESSAGE_GET_LIST, roleLevel: RoleLevel.ChatAdmin)]
    public async Task<IActionResult> ListWelcomeMessage(ListWelcomeMessageRequest request)
    {
        return await SendRequest(request);
    }

    [HttpGet("welcome-message/{WelcomeId}")]
    [AccessFilter(flag: Flag.REST_GROUP_WELCOME_MESSAGE_GET_DETAIL, roleLevel: RoleLevel.ChatAdmin)]
    public async Task<IActionResult> GetWelcomeMessage(GetWelcomeMessageRequest request)
    {
        return await SendRequest(request);
    }

    [HttpPost("welcome-message")]
    [AccessFilter(flag: Flag.REST_GROUP_WELCOME_MESSAGE_CREATE, roleLevel: RoleLevel.ChatAdmin)]
    public async Task<IActionResult> SaveWelcomeMessage(SaveWelcomeMessageRequest request)
    {
        return await SendRequest(request);
    }

    [HttpDelete("welcome-message")]
    [AccessFilter(flag: Flag.REST_GROUP_WELCOME_MESSAGE_DELETE, roleLevel: RoleLevel.ChatAdmin)]
    public async Task<IActionResult> DeleteWelcomeMessage(DeleteWelcomeMessageRequest request)
    {
        return await SendRequest(request);
    }

    [HttpPost("select-welcome-message")]
    [AccessFilter(flag: Flag.REST_GROUP_WELCOME_MESSAGE_SELECT, roleLevel: RoleLevel.ChatAdmin)]
    public async Task<IActionResult> SelectWelcomeMessage(SelectWelcomeMessageRequest request)
    {
        return await SendRequest(request);
    }
}