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

    public DeleteMessageRequestHandler(ILogger<DeleteMessageRequestHandler> logger)
    {
        _logger = logger;
        _logger = logger;
    }

    public async Task<ResponseBase> Handle(DeleteMessageRequestModel request, CancellationToken cancellationToken)
    {
        var chatId = request.ChatIdentifier;

        ResponseBase responseBase = new(request);

        if (chatId == 0) return responseBase;

        try
        {
            _logger.LogDebug("Deleting message {MessageId} from chat {ChatId}", request.MessageId, chatId);
            await responseBase.Bot.DeleteMessageAsync(chatId, request.MessageId, cancellationToken: cancellationToken);

            _logger.LogInformation("Message {MessageId} deleted from chat {ChatId}", request.MessageId, chatId);
        }
        catch (Exception exception)
        {
            if (exception.Message.CanBeIgnored())
            {
                _logger.LogWarning("Message {MessageId} could not be deleted from chat {ChatId}", request.MessageId, chatId);
            }
            else
            {
                _logger.LogError(exception, "Message {MessageId} could not be deleted from chat {ChatId}", request.MessageId, chatId);
                throw;
            }
        }

        return responseBase.Complete();
    }
}