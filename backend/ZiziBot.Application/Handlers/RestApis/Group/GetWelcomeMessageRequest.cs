using Microsoft.AspNetCore.Mvc;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.Group;

public class GetWelcomeMessageRequest : ApiRequestBase<List<GetWelcomeMessageResponse>>
{
    [FromQuery]
    public long ChatId { get; set; }
}

public class GetWelcomeMessageResponse
{
    public long ChatId { get; set; }
    public string Text { get; set; }
    public string RawButton { get; set; }
    public string Media { get; set; }
    public int DataType { get; set; }
    public int Status { get; set; }
}

public class GetWelcomeMessageHandler : IRequestHandler<GetWelcomeMessageRequest, ApiResponseBase<List<GetWelcomeMessageResponse>>>
{
    private readonly GroupDbContext _groupDbContext;

    public GetWelcomeMessageHandler(GroupDbContext groupDbContext)
    {
        _groupDbContext = groupDbContext;
    }

    public async Task<ApiResponseBase<List<GetWelcomeMessageResponse>>> Handle(GetWelcomeMessageRequest request, CancellationToken cancellationToken)
    {
        var response = new ApiResponseBase<List<GetWelcomeMessageResponse>>();

        var query = await _groupDbContext.WelcomeMessage
            .AsNoTracking()
            .WhereIf(request.ChatId != 0, entity => entity.ChatId == request.ChatId)
            .Where(entity => request.ListChatId.Contains(entity.ChatId))
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .ToListAsync(cancellationToken: cancellationToken);

        var data = query.Select(entity => new GetWelcomeMessageResponse
        {
            ChatId = entity.ChatId,
            Text = entity.Text,
            RawButton = entity.RawButton,
            Media = entity.Media,
            DataType = entity.DataType,
            Status = entity.Status,
        }).ToList();

        return response.Success("Get Welcome Message successfully", data);
    }
}