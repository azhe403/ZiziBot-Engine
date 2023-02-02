using System.Net;
using FluentValidation;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.GlobalBan;

public class UndeleteGlobalBanApiRequest : ApiRequestBase<bool>
{
    public long UserId { get; set; }
}

public class UndeleteGlobalBanApiValidator : AbstractValidator<UndeleteGlobalBanApiRequest>
{
    public UndeleteGlobalBanApiValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}

public class UndeleteGlobalBanRequestHandler : IRequestHandler<UndeleteGlobalBanApiRequest, ApiResponseBase<bool>>
{
    private readonly AntiSpamDbContext _antiSpamDbContext;

    public UndeleteGlobalBanRequestHandler(AntiSpamDbContext antiSpamDbContext)
    {
        _antiSpamDbContext = antiSpamDbContext;
    }

    public async Task<ApiResponseBase<bool>> Handle(UndeleteGlobalBanApiRequest request, CancellationToken cancellationToken)
    {
        var ban = await _antiSpamDbContext.GlobalBan
            .FirstOrDefaultAsync(entity => entity.UserId == request.UserId, cancellationToken: cancellationToken);

        if (ban == null)
        {
            return new ApiResponseBase<bool>()
            {
                StatusCode = HttpStatusCode.NotModified,
                Data = true
            };
        }

        ban.Status = (int) EventStatus.Complete;

        await _antiSpamDbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponseBase<bool>()
        {
            Data = true,
            StatusCode = HttpStatusCode.OK
        };
    }
}