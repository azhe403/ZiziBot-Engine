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
    private readonly AntiSpamDbContext _antiSpamDbContext;

    public DeleteGlobalBanApiHandler(AntiSpamDbContext antiSpamDbContext)
    {
        _antiSpamDbContext = antiSpamDbContext;
    }

    public async Task<ApiResponseBase<bool>> Handle(DeleteGlobalBanApiRequest request, CancellationToken cancellationToken)
    {
        var response = new ApiResponseBase<bool>();

        var globalBanEntities = await _antiSpamDbContext.GlobalBan
            .Where(e => e.UserId == request.UserId)
            .ToListAsync(cancellationToken: cancellationToken);

        globalBanEntities.ForEach(entity => {
            entity.Status = (int)EventStatus.Deleted;
        });

        await _antiSpamDbContext.SaveChangesAsync(cancellationToken);

        return response.Success("Global ban deleted.", true);
    }
}