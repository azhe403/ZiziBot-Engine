using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace ZiziBot.Application.Handlers.Telegram.Core;

public class DeleteMessageBotRequestModel : BotRequestBase
{
    public int MessageId { get; set; }
}

[UsedImplicitly]
public class DeleteMessageRequestHandler(ILogger<DeleteMessageRequestHandler> logger, TelegramService telegramService)
    : IRequestHandler<DeleteMessageBotRequestModel, BotResponseBase>
{
    public async Task<BotResponseBase> Handle(DeleteMessageBotRequestModel request, CancellationToken cancellationToken)
    {
        var chatId = request.ChatIdentifier;

        telegramService.SetupResponse(request);

        if (chatId == 0)
            return telegramService.Complete();

        try
        {
            logger.LogDebug("Deleting message {MessageId} from chat {ChatId}", request.MessageId, chatId);
            await telegramService.Bot.DeleteMessageAsync(chatId, request.MessageId, cancellationToken: cancellationToken);

            logger.LogInformation("Message {MessageId} deleted from chat {ChatId}", request.MessageId, chatId);
        }
        catch (Exception exception)
        {
            if (exception.Message.IsIgnorable())
            {
                logger.LogWarning("Message {MessageId} could not be deleted from chat {ChatId}", request.MessageId, chatId);
            }
            else
            {
                logger.LogError(exception, "Message {MessageId} could not be deleted from chat {ChatId}", request.MessageId, chatId);
                throw;
            }
        }

        return telegramService.Complete();
    }
}