using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/mirror-user")]
public class MirrorUserController : ApiControllerBase
{
    [HttpGet()]
    [Authorize(Roles = "Sudoer")]
    [AccessFilter(flag: Flag.REST_MIRROR_USER_GET_LIST, checkHeader: true)]
    public async Task<IActionResult> GetUsersAll(GetMirrorUsersRequestDto request)
    {
        return await SendRequest(request);
    }

    [HttpGet("check-user")]
    [AccessFilter(flag: Flag.REST_MIRROR_USER_CHECK_USER, checkHeader: false)]
    public async Task<IActionResult> GetUserByUserId([FromQuery] GetMirrorUserByUserIdRequestDto request)
    {
        return await SendRequest(request);
    }

    [HttpPost()]
    [AccessFilter(flag: Flag.REST_MIRROR_USER_CREATE, checkHeader: true)]
    public async Task<IActionResult> PostUserMirror([FromBody] PostMirrorUserRequestDto request)
    {
        return await SendRequest(request);
    }

    [HttpDelete]
    [AccessFilter(flag: Flag.REST_MIRROR_USER_DELETE, checkHeader: true)]
    public async Task<IActionResult> DeleteMirrorUser([FromQuery] DeleteMirrorUserRequestDto request)
    {
        return await SendRequest(request);
    }

    [HttpPost("submit-donation")]
    [AccessFilter(flag: Flag.REST_MIRROR_SUBMIT_DONATION, checkHeader: true)]
    public async Task<IActionResult> SubmitDonation(SubmitDonationRequest request)
    {
        return await SendRequest(request);
    }

    [HttpGet("check-order")]
    [AccessFilter(flag: Flag.REST_MIRROR_USER_CHECK_ORDER, checkHeader: true)]
    public async Task<IActionResult> Index([FromQuery] string orderId)
    {
        return await SendRequest(new CheckPaymentOrderIdRequest() {
            OrderId = orderId
        });
    }

    [HttpPost("verify-user")]
    public async Task<IActionResult> VerifyUser(VerifyUserRequest request)
    {
        return await SendRequest(request);
    }

    [HttpPost("trakteer-webhook")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> WebHookTrakteer(WebHookTrakteerDonationRequest request)
    {
        return await SendRequest(request);
    }
}