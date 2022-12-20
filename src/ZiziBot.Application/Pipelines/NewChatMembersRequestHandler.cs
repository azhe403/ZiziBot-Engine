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
        ResponseBase responseBase = new(request);
        _logger.LogInformation("New Chat Members. ChatId: {ChatId}", request.Message.Chat.Id);

        var users = request.Message.NewChatMembers?
            .Select(user => (user.FirstName + " " + user.LastName).Trim())
            .Aggregate((s, next) => s + ", " + next);

        var message = "Hai " + users + "" +
                      "\nSelamat datang di Kontrakan " + request.ChatTTitle;

        await responseBase.SendMessageText(message);
        return responseBase.Complete();
    }
}