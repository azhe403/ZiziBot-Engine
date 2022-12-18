using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Pipelines;

public class NewChatMembersRequestModel : RequestBase
{
}

[UsedImplicitly]
internal class NewChatMembersRequestHandler : IRequestHandler<NewChatMembersRequestModel, ResponseBase>
{
    private readonly IMediator _mediator;
    private readonly ILogger<NewChatMembersRequestHandler> _logger;

    public NewChatMembersRequestHandler(IMediator mediator, ILogger<NewChatMembersRequestHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<ResponseBase> Handle(NewChatMembersRequestModel request, CancellationToken cancellationToken)
    {
        ResponseBase responseBase = new();
        _logger.LogInformation("New Chat Members. ChatId: {ChatId}", request.Message.Chat.Id);

        var message = "Hai " + request.Message.From.FirstName + " " + request.Message.From.LastName + " selamat datang di grup ";

        responseBase.Complete();

        await _mediator.EnqueueAsync(new SendMessageTextRequestModel()
        {
            Message = request.Message,
            BotData = request.BotData,
            ReplyToMessageId = request.Message.MessageId,
            DirectAction = request.DirectAction,
            Text = message
        });

        return responseBase;
    }
}