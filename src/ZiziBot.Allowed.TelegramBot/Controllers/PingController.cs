using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class PingController : CommandController
{
	private readonly IMediator _mediator;

	public PingController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[Command("ping")]
	public async Task Ping(MessageData data)
	{
		await _mediator.EnqueueAsync(
			new PingRequestModel()
			{
				Options = data.Options,
				Message = data.Message,
				DeleteAfter = TimeSpan.FromMinutes(1),
				ReplyToMessageId = data.Message.MessageId,
				DirectAction = true
			}
		);
	}
}