using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.Application.Handlers.RestApis.GlobalBan;

public class PostGlobalBanApiRequest : ApiRequestBase<bool>
{
    [FromBody]
    public PostGlobalBanApiModel Model { get; set; }
}

public class PostGlobalBanApiModel
{
    public long UserId { get; set; }
    public string? Reason { get; set; }
}

public class PostGlobalBanApiValidator : AbstractValidator<PostGlobalBanApiRequest>
{
    public PostGlobalBanApiValidator()
    {
        RuleFor(x => x.Model.UserId).GreaterThan(0).WithMessage("UserId must be greater than 0");
    }
}

public class PostGlobalBanApiHandler(
    DataFacade dataFacade
) : IApiRequestHandler<PostGlobalBanApiRequest, bool>
{
    public async Task<ApiResponseBase<bool>> Handle(PostGlobalBanApiRequest request, CancellationToken cancellationToken)
    {
        var response = new ApiResponseBase<bool>();

        var globalBan = await dataFacade.MongoEf.GlobalBan
            .Where(entity => entity.UserId == request.Model.UserId)
            .Where(entity => entity.Status == EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (globalBan != null)
        {
            globalBan.Reason = request.Model.Reason;
        }
        else
        {
            dataFacade.MongoEf.GlobalBan.Add(new GlobalBanEntity() {
                UserId = request.Model.UserId,
                Reason = request.Model.Reason,
                Status = EventStatus.Complete
            });
        }

        await dataFacade.MongoEf.SaveChangesAsync(cancellationToken);

        return response.Success("Global ban saved.", true);
    }
}