using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace ZiziBot.Application.Handlers.Telegram.Group;

public class NewChatMembersRequestModel : RequestBase
{
    public User[] NewUser { get; set; }
}

[UsedImplicitly]
internal class NewChatMembersRequestHandler : IRequestHandler<NewChatMembersRequestModel, ResponseBase>
{
    private readonly ILogger<NewChatMembersRequestHandler> _logger;
    private readonly TelegramService _telegramService;

    public NewChatMembersRequestHandler(ILogger<NewChatMembersRequestHandler> logger, TelegramService telegramService)
    {
        _logger = logger;
        _telegramService = telegramService;
    }

    public async Task<ResponseBase> Handle(NewChatMembersRequestModel request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);
        _logger.LogInformation("New Chat Members. ChatId: {ChatId}", request.ChatId);

        var users = request.NewUser
            .Select(user => user.GetFullMention())
            .Aggregate((s, next) => s + ", " + next);

        var message = $"Hai {users}\n" +
                      $"Selamat datang di Kontrakan {request.ChatTitle}";

        await _telegramService.SendMessageText(message);
        return _telegramService.Complete();
    }
}