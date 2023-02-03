using System.Net;
using FluentValidation;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.GlobalBan;

public class PostGlobalBanApiRequest : ApiRequestBase<bool>
{
    public long UserId { get; set; }
    public string Reason { get; set; }
}

public class PostGlobalBanApiValidator : AbstractValidator<PostGlobalBanApiRequest>
{
    public PostGlobalBanApiValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}

public class PostGlobalBanRequestHandler : IRequestHandler<PostGlobalBanApiRequest, ApiResponseBase<bool>>
{
    private readonly AntiSpamDbContext _antiSpamDbContext;

    public PostGlobalBanRequestHandler(AntiSpamDbContext antiSpamDbContext)
    {
        _antiSpamDbContext = antiSpamDbContext;
    }

    public async Task<ApiResponseBase<bool>> Handle(PostGlobalBanApiRequest request, CancellationToken cancellationToken)
    {
        var ban = await _antiSpamDbContext.GlobalBan.FirstOrDefaultAsync(entity => entity.UserId == request.UserId);

        if (ban != null)
        {
            ban.Reason = request.Reason;
        }
        else
        {

            _antiSpamDbContext.GlobalBan.Add(
                new GlobalBanEntity()
                {
                    UserId = request.UserId,
                    Reason = request.Reason,
                    Status = (int) EventStatus.Complete
                }
            );
        }
        await _antiSpamDbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponseBase<bool>()
        {
            Data = true,
            StatusCode = HttpStatusCode.OK
        };
    }
}