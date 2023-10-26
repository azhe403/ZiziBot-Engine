using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Group;

public class ThreadUpdateBotRequest : BotRequestBase
{
}

public class ThreadUpdateHandler : IBotRequestHandler<ThreadUpdateBotRequest>
{
    private readonly MongoDbContextBase _mongoDbContext;
    private readonly TelegramService _telegramService;

    public ThreadUpdateHandler(TelegramService telegramService, MongoDbContextBase mongoDbContext)
    {
        _telegramService = telegramService;
        _mongoDbContext = mongoDbContext;
    }

    public async Task<BotResponseBase> Handle(ThreadUpdateBotRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        var prevTopicName = string.Empty;
        var htmlMessage = HtmlMessage.Empty;

        var findTopic = await _mongoDbContext.GroupTopic
            .Where(entity => entity.ChatId == request.ChatIdentifier)
            .Where(entity => entity.ThreadId == request.MessageThreadId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (!request.CreatedTopicName.IsNullOrEmpty())
            htmlMessage.Text("Topic telah dibuat").Br()
                .Bold("Name: ").Code(request.TopicName).Br();

        if (!request.EditedTopicName.IsNullOrEmpty())
            htmlMessage.Text("Topic berubah nama").Br()
                .Bold("Sesudah: ").Code(request.TopicName).Br();

        if (findTopic == null)
        {
            var entity = new GroupTopicEntity()
            {
                ChatId = request.ChatIdentifier,
                ThreadId = request.MessageThreadId,
                Status = (int)EventStatus.Complete
            };

            if (request.TopicName != null)
                entity.ThreadName = request.TopicName;

            _mongoDbContext.GroupTopic.Add(entity);
        }
        else
        {
            if (request.TopicName != null)
            {
                prevTopicName = findTopic.ThreadName;
                findTopic.ThreadName = request.TopicName;

                htmlMessage.Bold("Sebelum: ").Code(prevTopicName).Br();
            }
        }

        htmlMessage.Bold("Topic ID: ").Code(request.MessageThreadId.ToString());

        await _mongoDbContext.SaveChangesAsync(cancellationToken);

        return await _telegramService.SendMessageText(htmlMessage.ToString());
    }
}