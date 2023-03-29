using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.Note;

public class GetNoteRequest : ApiRequestBase<object>
{
    public long ChatId { get; set; }
}

public class GetNoteHandler : IRequestHandler<GetNoteRequest, ApiResponseBase<object>>
{
    private readonly ChatDbContext _chatDbContext;

    public GetNoteHandler(ChatDbContext chatDbContext)
    {
        _chatDbContext = chatDbContext;
    }

    public async Task<ApiResponseBase<object>> Handle(GetNoteRequest request, CancellationToken cancellationToken)
    {
        var response = new ApiResponseBase<object>();

        var notes = await _chatDbContext.Note
            .AsNoTracking()
            .WhereIf(request.ChatId != 0, entity => entity.ChatId == request.ChatId)
            .Where(entity => request.ListChatId.Contains(entity.ChatId))
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .OrderBy(entity => entity.Query)
            .ToListAsync(cancellationToken: cancellationToken);

        return response.Success("Success", notes);
    }
}