using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ZiziBot.WebApi.Controllers;

public class ApiControllerBase : ControllerBase
{
	protected IMediator Mediator => HttpContext.RequestServices.GetRequiredService<IMediator>();

}