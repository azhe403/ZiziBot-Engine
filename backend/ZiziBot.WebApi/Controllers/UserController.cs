using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ZiziBot.Application.UseCases.User;

namespace ZiziBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(CreateSessionOtpUseCase createSessionOtpUseCase) : ApiControllerBase
{
    [HttpGet("info")]
    [AccessFilter(flag: Flag.REST_USER_INFO_GET, checkHeader: true, needAuthenticated: true)]
    public async Task<IActionResult> GetUserInfo(GetUserInfoRequest request)
    {
        return await SendRequest(request);
    }

    [HttpPost("session/telegram")]
    [AccessFilter(flag: Flag.REST_USER_TELEGRAM_SESSION_CREATE, checkHeader: true)]
    [AllowAnonymous]
    public async Task<IActionResult> PostTelegramSession(ValidateTelegramSessionRequest request)
    {
        return await SendRequest(request);
    }

    [HttpPost("session/otp")]
    [AccessFilter(flag: Flag.REST_USER_SESSION_OTP_POST)]
    [AllowAnonymous]
    public async Task<IActionResult> PostOtpSession([FromBody] CreateSessionOtpRequest request)
    {
        return await SendRequest(() => createSessionOtpUseCase.Handle(request));
    }

    [HttpGet("list-group")]
    [AccessFilter(flag: Flag.REST_USER_GROUP_LIST, checkHeader: true)]
    [EnableRateLimiting(RateLimitingPolicy.API_LIST_RATE_LIMITING_KEY)]
    public async Task<IActionResult> GetListGroup([FromQuery] GetListGroupRequest request)
    {
        return await SendRequest(request);
    }
}