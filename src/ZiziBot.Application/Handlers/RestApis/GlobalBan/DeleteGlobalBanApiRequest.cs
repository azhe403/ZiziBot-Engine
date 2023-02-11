using System.Net;
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

public class DeleteGlobalBanRequestHandler : IRequestHandler<DeleteGlobalBanApiRequest, ApiResponseBase<bool>>
{
    private readonly AntiSpamDbContext _antiSpamDbContext;

    public DeleteGlobalBanRequestHandler(AntiSpamDbContext antiSpamDbContext)
    {
        _antiSpamDbContext = antiSpamDbContext;
    }

    public async Task<ApiResponseBase<bool>> Handle(DeleteGlobalBanApiRequest request, CancellationToken cancellationToken)
    {
        var globalBanEntities = await _antiSpamDbContext.GlobalBan
            .Where(e => e.UserId == request.UserId)
            .ToListAsync(cancellationToken: cancellationToken);

        globalBanEntities.ForEach(
            entity => {
                entity.Status = (int) EventStatus.Deleted;
            }
        );

        await _antiSpamDbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponseBase<bool>()
        {
            Result = true,
            StatusCode = HttpStatusCode.OK
        };
    }
}