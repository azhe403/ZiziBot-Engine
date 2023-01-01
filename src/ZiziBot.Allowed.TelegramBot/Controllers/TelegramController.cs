using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Extensions.Collections;
using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[Route("telegram")]
public class TelegramController : TelegramControllerBase
{
	public TelegramController(
		IServiceProvider serviceProvider,
		ControllersCollection controllersCollection,
		ClientsCollection clientsCollection
	) : base(serviceProvider, controllersCollection, clientsCollection)
	{
	}
}