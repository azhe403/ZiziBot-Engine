using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace ZiziBot.Application.Handlers.Telegram.Core;

public class DeleteMessageRequestModel : RequestBase
{
    public int MessageId { get; set; }
}

[UsedImplicitly]
public class DeleteMessageRequestHandler : IRequestHandler<DeleteMessageRequestModel, ResponseBase>
{
    private readonly ILogger<DeleteMessageRequestHandler> _logger;
    private readonly TelegramService _telegramService;

    public DeleteMessageRequestHandler(ILogger<DeleteMessageRequestHandler> logger, TelegramService telegramService)
    {
        _logger = logger;
        _telegramService = telegramService;
    }

    public async Task<ResponseBase> Handle(DeleteMessageRequestModel request, CancellationToken cancellationToken)
    {
        var chatId = request.ChatIdentifier;

        _telegramService.SetupResponse(request);

        if (chatId == 0)
            return _telegramService.Complete();

        try
        {
            _logger.LogDebug("Deleting message {MessageId} from chat {ChatId}", request.MessageId, chatId);
            await _telegramService.Bot.DeleteMessageAsync(chatId, request.MessageId, cancellationToken: cancellationToken);

            _logger.LogInformation("Message {MessageId} deleted from chat {ChatId}", request.MessageId, chatId);
        }
        catch (Exception exception)
        {
            if (exception.Message.IsIgnorable())
            {
                _logger.LogWarning("Message {MessageId} could not be deleted from chat {ChatId}", request.MessageId, chatId);
            }
            else
            {
                _logger.LogError(exception, "Message {MessageId} could not be deleted from chat {ChatId}", request.MessageId, chatId);
                throw;
            }
        }

        return _telegramService.Complete();
    }
}