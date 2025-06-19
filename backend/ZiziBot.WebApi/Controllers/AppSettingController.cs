using Microsoft.AspNetCore.Mvc;
using ZiziBot.Application.UseCases.AppSetting;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppSettingController(GetListAppSettingUseCase getListAppSettingUseCase) : ApiControllerBase
{
    [HttpGet]
    [AccessFilter(flag: Flag.REST_APP_SETTING_GET_LIST, apiRoleLevel: RoleLevel.Sudo)]
    public async Task<IActionResult> GetList(GetListAppSettingRequest request)
    {
        return await SendRequest(() => getListAppSettingUseCase.Handle(request));
    }
}