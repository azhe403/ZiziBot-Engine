namespace ZiziBot.Application.Handlers.Telegram.Group.Welcome;

public class ShowWelcomeMessageRequest : BotRequestBase
{

}

public class ShowWelcomeMessageHandler(TelegramService telegramService, MongoDbContextBase mongoDbContext, GroupRepository groupRepository)
    : IRequestHandler<ShowWelcomeMessageRequest, BotResponseBase>
{
    private readonly MongoDbContextBase _mongoDbContext = mongoDbContext;

    public async Task<BotResponseBase> Handle(ShowWelcomeMessageRequest request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        var welcomeMessage = await groupRepository.GetWelcomeMessage(request.ChatIdentifier);

        if (welcomeMessage == null)
        {
            return await telegramService.SendMessageAsync("Belum ada Welcome yang diatur, konfigurasi default akan diterapkan.");
        }

        return await telegramService.SendMessageAsync(
            text: welcomeMessage.Text,
            replyMarkup: welcomeMessage.RawButton.ToButtonMarkup(),
            fileId: welcomeMessage.Media,
            mediaType: (CommonMediaType)welcomeMessage.DataType
        );
    }
}