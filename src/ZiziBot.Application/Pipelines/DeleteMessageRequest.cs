using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace ZiziBot.Application.Pipelines;

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
        var chatId = request.Message.Chat.Id;
        var botToken = request.BotData.Token;

        ResponseBase responseBase = new(botToken);

        _logger.LogDebug("Deleting message {MessageId} from chat {ChatId}", request.MessageId, chatId);
        await responseBase.Bot.DeleteMessageAsync(chatId, request.MessageId, cancellationToken: cancellationToken);

        _logger.LogInformation("Message {MessageId} deleted from chat {ChatId}", request.MessageId, chatId);

        return responseBase.Complete();
    }
}