using System.Net;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.MirrorUser;

public class GetMirrorUserByUserIdRequestDto : ApiRequestBase<GetMirrorUserDto>
{
    public long UserId { get; set; }
}

public class GetMirrorUserDto
{
    public long UserId { get; set; }
    public bool HasSubscription { get; set; }
    public DateTime? SubscriptionUntil { get; set; }
    public DateTime? MemberSince { get; set; }
}

public class GetMirrorUserByUserIdRequestHandler : IRequestHandler<GetMirrorUserByUserIdRequestDto, ApiResponseBase<GetMirrorUserDto>>
{
    private readonly MongoDbContextBase _mongoDbContext;

    public GetMirrorUserByUserIdRequestHandler(MongoDbContextBase mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }

    public async Task<ApiResponseBase<GetMirrorUserDto>> Handle(GetMirrorUserByUserIdRequestDto request, CancellationToken cancellationToken)
    {
        ApiResponseBase<GetMirrorUserDto> response = new();

        var user = await _mongoDbContext.MirrorUsers
            .Where(x =>
                x.UserId == request.UserId &&
                x.Status == (int)EventStatus.Complete
            )
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (user == null)
        {
            response.StatusCode = HttpStatusCode.NotFound;
            response.Message = "Mirror User not found";

            return response;
        }
        else
        {
            response.StatusCode = HttpStatusCode.OK;
            response.Message = "Mirror User found";
            response.Result = new GetMirrorUserDto
            {
                UserId = request.UserId,
                HasSubscription = user.ExpireDate > DateTime.UtcNow.AddHours(Env.DEFAULT_TIMEZONE),
                SubscriptionUntil = user.ExpireDate.AddHours(Env.DEFAULT_TIMEZONE),
                MemberSince = user.CreatedDate.AddHours(Env.DEFAULT_TIMEZONE)
            };

            return response;
        }
    }
}