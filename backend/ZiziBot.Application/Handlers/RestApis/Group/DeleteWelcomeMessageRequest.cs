using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace ZiziBot.Application.Handlers.RestApis.Group;

public class DeleteWelcomeMessageRequest : ApiRequestBase<object>
{
    [FromBody]
    public DeleteWelcomeMessageRequestModel Model { get; set; }
}

public class DeleteWelcomeMessageValidation : AbstractValidator<DeleteWelcomeMessageRequest>
{
    public DeleteWelcomeMessageValidation()
    {
        RuleFor(x => x.Model.Id).NotEmpty().WithMessage("Id is required");
        RuleFor(x => x.Model.ChatId).NotEqual(0).WithMessage("ChatId is required");
    }
}

public class DeleteWelcomeMessageRequestModel
{
    public string Id { get; set; }
    public long ChatId { get; set; }

    [BindNever]
    [SwaggerIgnore]
    public ObjectId ObjectId => ObjectId.Parse(Id);
}

public class DeleteWelcomeMessageHandler(
    DataFacade dataFacade
) : IApiRequestHandler<DeleteWelcomeMessageRequest, object>
{
    public async Task<ApiResponseBase<object>> Handle(DeleteWelcomeMessageRequest request, CancellationToken cancellationToken)
    {
        var response = new ApiResponseBase<object>();

        if (!request.AdminChatId.Contains(request.Model.ChatId))
        {
            return response.BadRequest("You don't have access to this Group");
        }

        var findWelcome = await dataFacade.MongoEf.WelcomeMessage
            .Where(x => x.ChatId == request.Model.ChatId)
            .Where(x => x.Status != EventStatus.Deleted)
            .Where(x => x.Id == request.Model.ObjectId)
            .FirstOrDefaultAsync(x => x.Id == request.Model.ObjectId, cancellationToken);

        if (findWelcome == null)
        {
            return response.BadRequest("Welcome Message not found");
        }

        findWelcome.Status = EventStatus.Deleted;
        findWelcome.UserId = request.SessionUserId;
        findWelcome.TransactionId = request.TransactionId;

        await dataFacade.MongoEf.SaveChangesAsync(cancellationToken);

        return response.Success("Delete Welcome Message successfully", true);
    }
}