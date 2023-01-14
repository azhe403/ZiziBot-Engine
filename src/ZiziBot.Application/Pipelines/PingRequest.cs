using MediatR;
using Telegram.Bot;

namespace ZiziBot.Application.Pipelines;

public class PingRequestModel : RequestBase
{
}

public class PingRequestHandler : IRequestHandler<PingRequestModel, ResponseBase>
{

	public PingRequestHandler()
	{
	}

	public async Task<ResponseBase> Handle(PingRequestModel request, CancellationToken cancellationToken)
	{
		ResponseBase responseBase = new(request);

		var webhookInfo = await responseBase.Bot.GetWebhookInfoAsync(cancellationToken: cancellationToken);

		var htmlMessage = HtmlMessage.Empty
			.BoldBr("Pong!")
			.Br();

		if (!string.IsNullOrEmpty(webhookInfo.Url))
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