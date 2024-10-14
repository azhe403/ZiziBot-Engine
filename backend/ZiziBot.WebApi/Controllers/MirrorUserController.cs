using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/mirror-user")]
public class MirrorUserController : ApiControllerBase
{
    [HttpGet()]
    [Authorize(Roles = "Sudoer")]
    [AccessFilter(checkHeader: true)]
    public async Task<IActionResult> GetUsersAll(GetMirrorUsersRequestDto request)
    {
        return await SendRequest(request);
    }

    [HttpGet("find")]
    [HttpGet("check-user")]
    [AccessFilter(checkHeader: false)]
    public async Task<IActionResult> GetUserByUserId([FromQuery] GetMirrorUserByUserIdRequestDto request)
    {
        return await SendRequest(request);
    }

    [HttpPost()]
    [AccessFilter(checkHeader: true)]
    public async Task<IActionResult> PostUserMirror([FromBody] PostMirrorUserRequestDto request)
    {
        return await SendRequest(request);
    }

    [HttpDelete]
    [AccessFilter(checkHeader: true)]
    public async Task<IActionResult> DeleteMirrorUser([FromQuery] DeleteMirrorUserRequestDto request)
    {
        return await SendRequest(request);
    }

    [HttpPost("submit-payment-url")]
    [AccessFilter(checkHeader: true)]
    public async Task<IActionResult> SubmitPaymentUrl(SubmitPaymentUrlRequest request)
    {
        return await SendRequest(request);
    }

    [HttpGet("check-order")]
    [AccessFilter(checkHeader: true)]
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
}