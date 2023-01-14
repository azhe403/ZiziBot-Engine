using MediatR;
using Telegram.Bot;

namespace ZiziBot.Application.Pipelines;

public class PingRequestModel : RequestBase
{
}

public class PingRequestHandler : IRequestHandler<PingRequestModel, ResponseBase>
{
	private readonly SudoService _sudoService;

	public PingRequestHandler(SudoService sudoService)
	{
		_sudoService = sudoService;
	}

	public async Task<ResponseBase> Handle(PingRequestModel request, CancellationToken cancellationToken)
	{
		ResponseBase responseBase = new(request);

		var webhookInfo = await responseBase.Bot.GetWebhookInfoAsync(cancellationToken: cancellationToken);

		var htmlMessage = HtmlMessage.Empty
			.BoldBr("Pong!")
			.Br();

		if (!string.IsNullOrEmpty(webhookInfo.Url) &&
		    await _sudoService.IsSudoAsync(request.UserId))
		{
			htmlMessage
				.Bold("EngineMode: ").TextBr("WebHook")
				.Bold("URL: ").TextBr(webhookInfo.Url)
				.Bold("Custom Cert: ").TextBr(webhookInfo.HasCustomCertificate.ToString())
				.Bold("Allowed Updates: ").TextBr(webhookInfo.AllowedUpdates?.ToString())
				.Bold("Pending Count: ").TextBr((webhookInfo.PendingUpdateCount - 1).ToString())
				.Bold("Max Connection: ").TextBr(webhookInfo.MaxConnections.ToString())
				.Bold("Last Error: ").TextBr(webhookInfo.LastErrorDate?.ToString())
				.Bold("Error Message: ").TextBr(webhookInfo.LastErrorMessage)
				.Br();
		}

		return await responseBase.SendMessageText(htmlMessage.ToString());
	}
}