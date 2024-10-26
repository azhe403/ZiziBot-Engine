using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Behaviours;

public class EnsureChatSettingBehavior<TRequest, TResponse>(
    ILogger<EnsureChatSettingBehavior<TRequest, TResponse>> logger,
    DataFacade dataFacade
)
    : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : BotRequestBase, IRequest<TResponse>
    where TResponse : BotResponseBase
{
    public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        if (request.ChatId == 0)
            return;

        await dataFacade.ChatSetting.RefreshChatInfo(new() {
            ChatId = request.ChatIdentifier,
            ChatTitle = request.ChatTitle,
            ChatType = request.ChatType
        });

        logger.LogInformation("Ensure ChatSetting for ChatId: {ChatId} Done", request.ChatId);
    }
}