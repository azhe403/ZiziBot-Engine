using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ZiziBot.WebApi.Controllers;

public class ApiControllerBase : ControllerBase
{
    protected IMediator Mediator => HttpContext.RequestServices.GetRequiredService<IMediator>();
    protected MediatorService MediatorService => HttpContext.RequestServices.GetRequiredService<MediatorService>();

    protected IActionResult SwitchStatus<T>(ApiResponseBase<T> responseBase)
    {
        responseBase.transactionId = HttpContext.Request.Headers["transactionId"].ToString();

        return responseBase.StatusCode switch
        {
            HttpStatusCode.OK => Ok(responseBase),
            HttpStatusCode.BadRequest => BadRequest(responseBase),
            HttpStatusCode.NotFound => NotFound(responseBase),
            _ => BadRequest(responseBase)
        };
    }
}