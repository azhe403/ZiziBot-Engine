using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace ZiziBot.Application.Handlers.Telegram.Core;

public class SendMessageTextRequestModel : RequestBase
{
}

[UsedImplicitly]
[Obsolete("Send Message via ResponseBase")]
public class SendMessageTextRequestHandler : IRequestHandler<SendMessageTextRequestModel, ResponseBase>
{
    private readonly ILogger<SendMessageTextRequestHandler> _logger;
    private readonly IMediator _mediator;
    private readonly TelegramService _telegramService;

    public SendMessageTextRequestHandler(ILogger<SendMessageTextRequestHandler> logger, IMediator mediator, TelegramService telegramService)
    {
        _logger = logger;
        _mediator = mediator;
        _telegramService = telegramService;
    }

    public async Task<ResponseBase> Handle(SendMessageTextRequestModel request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        _logger.LogDebug("Sending message to chat {ChatId}", request.ChatId);
        var sentMessage = await _telegramService.Bot.SendTextMessageAsync(
            chatId: request.ChatId,
            text: request.Text,
            replyToMessageId: request.ReplyToMessageId,
            cancellationToken: cancellationToken
        );

        _logger.LogInformation("Message sent to chat {ChatId}", request.ChatId);

        if (request.DeleteAfter.Ticks <= 0)
            return _telegramService.Complete();

        _logger.LogDebug("Deleting message {MessageId} in {DeleteAfter} seconds", sentMessage.MessageId, request.DeleteAfter.TotalSeconds);
        _mediator.Schedule(
            new DeleteMessageRequestModel()
            {
                BotToken = request.BotToken,
                Message = request.Message,
                MessageId = sentMessage.MessageId,
                ExecutionStrategy = request.ExecutionStrategy,
                DeleteAfter = request.DeleteAfter
            }
        );

        _logger.LogInformation("Message {MessageId} scheduled for deletion in {DeleteAfter} seconds", sentMessage.MessageId, request.DeleteAfter.TotalSeconds);

        return _telegramService.Complete();
    }
}