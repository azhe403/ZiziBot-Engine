using MongoFramework.Linq;
using ZiziBot.DataSource.MongoDb.Entities;

namespace ZiziBot.Application.Handlers.Telegram.Group.Welcome;

public class SaveWelcomeMessageRequest : BotRequestBase
{
}

public class SaveWelcomeMessageHandler : IRequestHandler<SaveWelcomeMessageRequest, BotResponseBase>
{
    private readonly TelegramService _telegramService;
    private readonly MongoDbContextBase _mongoDbContext;
    private readonly GroupRepository _groupRepository;

    public SaveWelcomeMessageHandler(TelegramService telegramService, MongoDbContextBase mongoDbContext,
        GroupRepository groupRepository)
    {
        _telegramService = telegramService;
        _mongoDbContext = mongoDbContext;
        _groupRepository = groupRepository;
    }

    public async Task<BotResponseBase> Handle(SaveWelcomeMessageRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        if (request.ReplyToMessage == null)
        {
            return await _telegramService.SendMessageAsync("Balas pesan untuk disimpan");
        }

        await _telegramService.SendMessageAsync("Sedang menyimpan..");

        var welcomeMessage = await _mongoDbContext.WelcomeMessage
            .Where(e => e.ChatId == request.ChatIdentifier)
            .Where(e => e.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync();

        if (welcomeMessage == null)
        {
            _mongoDbContext.WelcomeMessage.Add(new WelcomeMessageEntity() {
                ChatId = request.ChatIdentifier,
                UserId = request.User!.Id,
                Status = (int)EventStatus.Complete,
            });

            await _mongoDbContext.SaveChangesAsync(cancellationToken);

            welcomeMessage = await _groupRepository.GetWelcomeMessage(request.ChatIdentifier);
        }

        switch (request.Command)
        {
            case "/wt":
                welcomeMessage.Text = request.ReplyToMessage.Text;
                break;
            case "/wd":
                welcomeMessage.Media = request.ReplyToMessage.GetFileId();
                welcomeMessage.DataType = (int)request.ReplyToMessage.Type;
                break;
            case "/wb":
                welcomeMessage.RawButton = request.ReplyToMessage.Text;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        await _mongoDbContext.SaveChangesAsync(cancellationToken);

        return await _telegramService.SendMessageAsync($"Berhasil menyimpan. {request.Command}");
    }
}