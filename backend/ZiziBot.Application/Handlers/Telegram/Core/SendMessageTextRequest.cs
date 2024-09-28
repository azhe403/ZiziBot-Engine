using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace ZiziBot.Application.Handlers.Telegram.Core;

public class SendMessageTextBotRequestModel : BotRequestBase
{
}

[UsedImplicitly]
[Obsolete("Send Message via ResponseBase")]
public class SendMessageTextRequestHandler(
    ILogger<SendMessageTextRequestHandler> logger,
    MediatorService mediatorService,
    TelegramService telegramService)
    : IRequestHandler<SendMessageTextBotRequestModel, BotResponseBase>
{
    public async Task<BotResponseBase> Handle(SendMessageTextBotRequestModel request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        logger.LogDebug("Sending message to chat {ChatId}", request.ChatId);
        var sentMessage = await telegramService.Bot.SendTextMessageAsync(
            chatId: request.ChatId,
            text: request.Text,
            replyToMessageId: request.ReplyToMessageId,
            cancellationToken: cancellationToken
        );

        logger.LogInformation("Message sent to chat {ChatId}", request.ChatId);

        if (request.DeleteAfter.Ticks <= 0)
            return telegramService.Complete();

        logger.LogDebug("Deleting message {MessageId} in {DeleteAfter} seconds", sentMessage.MessageId, request.DeleteAfter.TotalSeconds);
        mediatorService.Schedule(
            new DeleteMessageBotRequestModel() {
                BotToken = request.BotToken,
                Message = request.Message,
                MessageId = sentMessage.MessageId,
                ExecutionStrategy = request.ExecutionStrategy,
                DeleteAfter = request.DeleteAfter
            }
        );

        logger.LogInformation("Message {MessageId} scheduled for deletion in {DeleteAfter} seconds", sentMessage.MessageId,
            request.DeleteAfter.TotalSeconds);

        return telegramService.Complete();
    }
}