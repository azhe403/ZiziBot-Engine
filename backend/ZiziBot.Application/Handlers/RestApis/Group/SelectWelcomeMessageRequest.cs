using Microsoft.AspNetCore.Mvc;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.Group;

public class SelectWelcomeMessageRequest : ApiRequestBase<object>
{
    [FromBody]
    public SelectWelcomeMessageRequestModel Model { get; set; }
}

public class SelectWelcomeMessageRequestModel
{
    public long ChatId { get; set; }
    public string WelcomeId { get; set; }
}

public class SelectWelcomeMessageHandler : IRequestHandler<SelectWelcomeMessageRequest, ApiResponseBase<object>>
{
    private readonly GroupDbContext _groupDbContext;

    public SelectWelcomeMessageHandler(GroupDbContext groupDbContext)
    {
        _groupDbContext = groupDbContext;
    }

    public async Task<ApiResponseBase<object>> Handle(SelectWelcomeMessageRequest request, CancellationToken cancellationToken)
    {
        var response = new ApiResponseBase<object>();

        var listWelcomeMessage = await _groupDbContext.WelcomeMessage
            .Where(entity => request.ListChatId.Contains(entity.ChatId))
            .ToListAsync(cancellationToken: cancellationToken);

        if (listWelcomeMessage.Count == 0)
        {
            return response.BadRequest("Welcome Message not found", null);
        }

        listWelcomeMessage.ForEach(row => {
            row.Status = (int)EventStatus.Inactive;
        });

        var selectedWelcome = listWelcomeMessage.FirstOrDefault(entity => entity.Id.ToString() == request.Model.WelcomeId);

        if (selectedWelcome == null)
        {
            return response.BadRequest("Welcome Message not found", null);
        }

        selectedWelcome.Status = (int)EventStatus.Complete;

        await _groupDbContext.SaveChangesAsync(cancellationToken);

        return response.Success("Success", null);
    }
}