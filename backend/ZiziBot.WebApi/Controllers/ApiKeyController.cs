using Microsoft.AspNetCore.Mvc;
using ZiziBot.Application.UseCases.ApiKey;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiKeyController(GetListApiKeyUseCase getListApiKeyUseCase) : ApiControllerBase
{
    [HttpGet]
    [AccessFilter(flag: Flag.REST_API_KEY_GET, apiRoleLevel: RoleLevel.Sudo, needAuthenticated: true)]
    public async Task<IActionResult> Index(GetListApiKeyRequest request)
    {
        return await SendRequest(() => getListApiKeyUseCase.Handle(request));
    }
}