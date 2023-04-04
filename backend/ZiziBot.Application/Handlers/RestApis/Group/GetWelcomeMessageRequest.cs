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
    public string Id { get; set; }
    public long ChatId { get; set; }
    public string Text { get; set; }
    public string RawButton { get; set; }
    public string Media { get; set; }
    public int DataType { get; set; }
    public string DataTypeName { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; }
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
            .ToListAsync(cancellationToken: cancellationToken);

        var data = query.Select(entity => new GetWelcomeMessageResponse
        {
            Id = entity.Id.ToString(),
            ChatId = entity.ChatId,
            Text = entity.Text,
            RawButton = entity.RawButton,
            Media = entity.Media,
            DataType = entity.DataType,
            DataTypeName = ((CommonMediaType)entity.DataType).ToString(),
            Status = entity.Status,
            StatusName = ((EventStatus)entity.Status).ToString()
        }).ToList();

        return response.Success("Get Welcome Message successfully", data);
    }
}