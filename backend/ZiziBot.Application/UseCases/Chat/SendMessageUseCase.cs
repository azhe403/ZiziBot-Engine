using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.Application.UseCases.Chat;

public class SendMessageRequest
{
    public string BotToken { get; set; }
    public long ChatId { get; set; }
    public int? ThreadId { get; set; }
    public string Text { get; set; }
    public ReplyParameters? ReplyToMessageId { get; set; }
    public TimeSpan DeleteAfter { get; set; }
}

public class SendMessageUseCase(
    ILogger<SendMessageUseCase> logger,
    DataFacade dataFacade,
    ServiceFacade serviceFacade
)
{
    public async Task Handle(SendMessageRequest request)
    {
        logger.LogDebug("Sending a message to chat {ChatId}", request.ChatId);
        var botClient = new TelegramBotClient(request.BotToken);

        var sentMessage = await botClient.SendMessage(
            chatId: request.ChatId,
            text: request.Text,
            messageThreadId: request.ThreadId,
            replyParameters: request.ReplyToMessageId,
            parseMode: ParseMode.Html
        );

        logger.LogInformation("Message sent to chat {ChatId}", request.ChatId);

        if (request.DeleteAfter.Ticks <= 0)
            return;

        logger.LogDebug("Deleting message {MessageId} in {DeleteAfter} seconds", sentMessage.MessageId, request.DeleteAfter.TotalSeconds);
        HangfireUtil.Schedule<DeleteMessageUseCase>(x => x.Handle(new DeleteMessageBotRequest {
            BotToken = request.BotToken,
            MessageId = sentMessage.MessageId,
            ChatId = request.ChatId
        }), request.DeleteAfter);

        logger.LogInformation("Message {MessageId} scheduled for deletion in {DeleteAfter} seconds", sentMessage.MessageId, request.DeleteAfter.TotalSeconds);
    }
}