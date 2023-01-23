using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ZiziBot.WebApi.Controllers;

public class ApiControllerBase : ControllerBase
{
    protected IMediator Mediator => HttpContext.RequestServices.GetRequiredService<IMediator>();

    protected IActionResult SwitchStatus<T>(ApiResponseBase<T> responseBase)
    {
        switch (responseBase.StatusCode)
        {
            case HttpStatusCode.OK:
                return Ok(responseBase.Data);
                break;
            case HttpStatusCode.BadRequest:
                return BadRequest(responseBase.Data);
            default:
                return BadRequest(responseBase.Data);
                break;
        }
    }

}