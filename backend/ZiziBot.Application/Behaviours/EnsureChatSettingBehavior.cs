using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Behaviours;

public class EnsureChatSettingBehavior<TRequest, TResponse>(
    ILogger<EnsureChatSettingBehavior<TRequest, TResponse>> logger,
    DataFacade dataFacade,
    ServiceFacade serviceFacade
)
    : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : BotRequestBase, IRequest<TResponse>
    where TResponse : BotResponseBase
{
    public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        if (request.ChatId == 0 ||
            request.Source != ResponseSource.Bot)
            return;

        serviceFacade.TelegramService.SetupResponse(request);

        var memberCount = await serviceFacade.TelegramService.GetMemberCount();

        await dataFacade.ChatSetting.RefreshChatInfo(new() {
            ChatId = request.ChatIdentifier,
            ChatType = request.ChatType,
            MemberCount = request.IsGroup ? memberCount : -1,
            ChatTitle = request.ChatTitle,
            ChatUsername = request.Chat?.Username,
            TransactionId = request.TransactionId
        });

        logger.LogInformation("Ensure ChatSetting for ChatId: {ChatId} Done", request.ChatId);
    }
}