using FluentValidation;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.GlobalBan;

public class DeleteGlobalBanApiRequest : ApiRequestBase<bool>
{
    public long UserId { get; set; }
}

public class DeleteGlobalBanApiValidator : AbstractValidator<DeleteGlobalBanApiRequest>
{
    public DeleteGlobalBanApiValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}

public class DeleteGlobalBanApiHandler : IRequestHandler<DeleteGlobalBanApiRequest, ApiResponseBase<bool>>
{
    private readonly MongoDbContextBase _mongoDbContext;

    public DeleteGlobalBanApiHandler(MongoDbContextBase mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }

    public async Task<ApiResponseBase<bool>> Handle(DeleteGlobalBanApiRequest request, CancellationToken cancellationToken)
    {
        var response = new ApiResponseBase<bool>();

        var globalBanEntities = await _mongoDbContext.GlobalBan
            .Where(e => e.UserId == request.UserId)
            .ToListAsync(cancellationToken: cancellationToken);

        globalBanEntities.ForEach(entity => {
            entity.Status = (int)EventStatus.Deleted;
        });

        await _mongoDbContext.SaveChangesAsync(cancellationToken);

        return response.Success("Global ban deleted.", true);
    }
}