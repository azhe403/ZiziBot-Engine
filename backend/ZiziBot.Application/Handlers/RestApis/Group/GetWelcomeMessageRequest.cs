using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.Group;

public class GetWelcomeMessageRequest : ApiRequestBase<WelcomeMessageResponse>
{
    [FromRoute]
    public string WelcomeId { get; set; }
}

public class GetWelcomeMessageHandler : IRequestHandler<GetWelcomeMessageRequest, ApiResponseBase<WelcomeMessageResponse>>
{
    private readonly GroupDbContext _groupDbContext;

    public GetWelcomeMessageHandler(GroupDbContext groupDbContext)
    {
        _groupDbContext = groupDbContext;
    }

    public async Task<ApiResponseBase<WelcomeMessageResponse>> Handle(GetWelcomeMessageRequest request, CancellationToken cancellationToken)
    {
        var response = new ApiResponseBase<WelcomeMessageResponse>();

        var query = await _groupDbContext.WelcomeMessage
            .AsNoTracking()
            .Where(entity => entity.Id == new ObjectId(request.WelcomeId))
            .Where(entity => request.ListChatId.Contains(entity.ChatId))
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (query == null)
        {
            return response.BadRequest("Welcome Message not found");
        }

        var data = new WelcomeMessageResponse
        {
            Id = query.Id.ToString(),
            ChatId = query.ChatId,
            Text = query.Text,
            RawButton = query.RawButton,
            Media = query.Media,
            DataType = query.DataType,
            DataTypeName = ((CommonMediaType)query.DataType).ToString(),
            Status = query.Status,
            StatusName = ((EventStatus)query.Status).ToString()
        };

        return response.Success("Get Welcome Message successfully", data);
    }
}