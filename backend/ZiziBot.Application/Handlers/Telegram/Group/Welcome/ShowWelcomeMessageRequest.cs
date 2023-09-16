namespace ZiziBot.Application.Handlers.Telegram.Group.Welcome;

public class ShowWelcomeMessageRequest : BotRequestBase
{

}

public class ShowWelcomeMessageHandler : IRequestHandler<ShowWelcomeMessageRequest, BotResponseBase>
{
    private readonly TelegramService _telegramService;
    private readonly MongoDbContextBase _mongoDbContext;
    private readonly GroupRepository _groupRepository;

    public ShowWelcomeMessageHandler(TelegramService telegramService, MongoDbContextBase mongoDbContext, GroupRepository groupRepository)
    {
        _telegramService = telegramService;
        _mongoDbContext = mongoDbContext;
        _groupRepository = groupRepository;
    }

    public async Task<BotResponseBase> Handle(ShowWelcomeMessageRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        var welcomeMessage = await _groupRepository.GetWelcomeMessage(request.ChatIdentifier);

        if (welcomeMessage == null)
        {
            return await _telegramService.SendMessageAsync("Belum ada Welcome yang diatur, konfigurasi default akan diterapkan.");
        }

        return await _telegramService.SendMessageAsync(
            text: welcomeMessage.Text,
            replyMarkup: welcomeMessage.RawButton.ToButtonMarkup(),
            fileId: welcomeMessage.Media,
            mediaType: (CommonMediaType)welcomeMessage.DataType
        );
    }
}