namespace ZiziBot.Application.Handlers.Telegram.Chat;

public class ChatJoinBotRequest : BotRequestBase
{
}

public class ChatJoinBotHandler(TelegramService telegramService) : IRequestHandler<ChatJoinBotRequest, BotResponseBase>
{
    public async Task<BotResponseBase> Handle(ChatJoinBotRequest request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        await telegramService.AnswerJoinRequestAsync(request.ChatJoinRequest);


        return telegramService.Complete();
    }
}