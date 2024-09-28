using MongoFramework.Linq;
using ZiziBot.DataSource.MongoDb.Entities;

namespace ZiziBot.Application.Handlers.Telegram.Group.Welcome;

public class SaveWelcomeMessageRequest : BotRequestBase
{
}

public class SaveWelcomeMessageHandler(
    TelegramService telegramService,
    MongoDbContextBase mongoDbContext,
    GroupRepository groupRepository)
    : IRequestHandler<SaveWelcomeMessageRequest, BotResponseBase>
{
    public async Task<BotResponseBase> Handle(SaveWelcomeMessageRequest request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        if (request.ReplyToMessage == null)
        {
            return await telegramService.SendMessageAsync("Balas pesan untuk disimpan");
        }

        await telegramService.SendMessageAsync("Sedang menyimpan..");

        var welcomeMessage = await mongoDbContext.WelcomeMessage
            .Where(e => e.ChatId == request.ChatIdentifier)
            .Where(e => e.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync();

        if (welcomeMessage == null)
        {
            mongoDbContext.WelcomeMessage.Add(new WelcomeMessageEntity() {
                ChatId = request.ChatIdentifier,
                UserId = request.User!.Id,
                Status = (int)EventStatus.Complete,
            });

            await mongoDbContext.SaveChangesAsync(cancellationToken);

            welcomeMessage = await groupRepository.GetWelcomeMessage(request.ChatIdentifier);
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

        await mongoDbContext.SaveChangesAsync(cancellationToken);

        return await telegramService.SendMessageAsync($"Berhasil menyimpan. {request.Command}");
    }
}