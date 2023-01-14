using MediatR;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.Application.Pipelines;

public class GetIdRequestModel : RequestBase
{
}

public class GetIdRequestHandler : IRequestHandler<GetIdRequestModel, ResponseBase>
{
	public async Task<ResponseBase> Handle(GetIdRequestModel request, CancellationToken cancellationToken)
	{
		ResponseBase responseBase = new(request);

		var htmlMessage = HtmlMessage.Empty;

		if (request.ChatType != ChatType.Private)
		{
			htmlMessage.BoldBr($"👥 {request.ChatTitle}")
				.Bold("Chat ID: ").CodeBr(request.ChatId.ToString())
				.Br();
		}

		htmlMessage.BoldBr($"👤 {request.UserFullName}")
			.Bold("User ID: ").CodeBr(request.UserId.ToString());

		return await responseBase.SendMessageText(htmlMessage.ToString());
	}
}