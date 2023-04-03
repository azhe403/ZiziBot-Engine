using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.Application.Handlers.RestApis.Group;

public class SaveWelcomeMessageRequest : ApiRequestBase<object>
{
    [FromBody]
    public SaveWelcomeMessageRequestModel Model { get; set; }
}

public class SaveWelcomeMessageRequestModel
{
    public long ChatId { get; set; }
    public string Text { get; set; }
    public string? RawButton { get; set; }
    public string? Media { get; set; }
    public int DataType { get; set; } = -1;
}

public class SaveWelcomeMessageHandler : IRequestHandler<SaveWelcomeMessageRequest, ApiResponseBase<object>>
{
    private readonly GroupDbContext _groupDbContext;

    public SaveWelcomeMessageHandler(GroupDbContext groupDbContext)
    {
        _groupDbContext = groupDbContext;
    }

    public async Task<ApiResponseBase<object>> Handle(SaveWelcomeMessageRequest request, CancellationToken cancellationToken)
    {
        var response = new ApiResponseBase<object>();

        _groupDbContext.WelcomeMessage.Add(new WelcomeMessageEntity
        {
            ChatId = request.Model.ChatId,
            Text = request.Model.Text,
            RawButton = request.Model.RawButton,
            Media = request.Model.Media,
            DataType = request.Model.DataType,
            Status = (int)EventStatus.Complete,
            TransactionId = request.TransactionId
        });

        await _groupDbContext.SaveChangesAsync(cancellationToken);

        return response.Success("Success", null);
    }
}