using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Group;

public class ThreadUpdateRequest : RequestBase
{
}

public class ThreadUpdateHandler : IRequestHandler<ThreadUpdateRequest, ResponseBase>
{
    private readonly GroupDbContext _groupDbContext;
    private readonly TelegramService _telegramService;

    public ThreadUpdateHandler(TelegramService telegramService, GroupDbContext groupDbContext)
    {
        _telegramService = telegramService;
        _groupDbContext = groupDbContext;
    }

    public async Task<ResponseBase> Handle(ThreadUpdateRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        var prevTopicName = string.Empty;
        var htmlMessage = HtmlMessage.Empty;

        var findTopic = await _groupDbContext.GroupTopic
            .Where(entity => entity.ChatId == request.ChatIdentifier)
            .Where(entity => entity.ThreadId == request.MessageThreadId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if(!request.CurrentTopicName.IsNullOrEmpty())
            htmlMessage.Text("Topic berubah nama menjadi ").BoldBr($" {request.CurrentTopicName}");

        if (findTopic == null)
        {
            var entity = new GroupTopicEntity()
            {
                ChatId = request.ChatIdentifier,
                ThreadId = request.MessageThreadId,
                Status = (int)EventStatus.Complete
            };

            if (request.CurrentTopicName != null)
                entity.ThreadName = request.CurrentTopicName;

            _groupDbContext.GroupTopic.Add(entity);
        }
        else
        {
            if (request.CurrentTopicName != null)
            {
                prevTopicName = findTopic.ThreadName;
                findTopic.ThreadName = request.CurrentTopicName;

                htmlMessage.Text("Sebelumnya ").Bold(prevTopicName);
            }
        }

        await _groupDbContext.SaveChangesAsync(cancellationToken);

        return await _telegramService.SendMessageText(htmlMessage.ToString());
    }
}