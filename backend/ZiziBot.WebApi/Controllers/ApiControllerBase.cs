using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ZiziBot.WebApi.Controllers;

public class ApiControllerBase : ControllerBase
{
    protected IMediator Mediator => HttpContext.RequestServices.GetRequiredService<IMediator>();
    protected MediatorService MediatorService => HttpContext.RequestServices.GetRequiredService<MediatorService>();

    protected async Task<IActionResult> SendRequest<T>(ApiRequestBase<T> request)
    {
        var result = await Mediator.Send(request);
        return SwitchStatus(result);
    }

    protected IActionResult SwitchStatus<T>(ApiResponseBase<T> responseBase)
    {
        responseBase.TransactionId = HttpContext.Request.Headers.GetTransactionId();

        return responseBase.StatusCode switch
        {
            HttpStatusCode.OK => Ok(responseBase),
            HttpStatusCode.BadRequest => BadRequest(responseBase),
            HttpStatusCode.Unauthorized => Unauthorized(responseBase),
            HttpStatusCode.NotFound => NotFound(responseBase),
            _ => BadRequest(responseBase)
        };
    }

}