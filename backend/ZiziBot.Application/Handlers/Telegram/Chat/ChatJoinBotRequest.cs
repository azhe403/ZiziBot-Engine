namespace ZiziBot.Application.Handlers.Telegram.Chat;

public class ChatJoinBotRequest : BotRequestBase
{
}

public class ChatJoinBotHandler : IRequestHandler<ChatJoinBotRequest, BotResponseBase>
{
    private readonly TelegramService _telegramService;

    public ChatJoinBotHandler(TelegramService telegramService)
    {
        _telegramService = telegramService;

    }

    public async Task<BotResponseBase> Handle(ChatJoinBotRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        await _telegramService.AnswerJoinRequestAsync(request.ChatJoinRequest);


        return _telegramService.Complete();
    }
}