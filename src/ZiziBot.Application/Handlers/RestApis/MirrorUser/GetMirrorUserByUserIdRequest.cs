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
    private readonly MirrorDbContext _mirrorDbContext;

    public GetMirrorUserByUserIdRequestHandler(MirrorDbContext mirrorDbContext)
    {
        _mirrorDbContext = mirrorDbContext;
    }

    public async Task<ApiResponseBase<GetMirrorUserDto>> Handle(GetMirrorUserByUserIdRequestDto request, CancellationToken cancellationToken)
    {
        ApiResponseBase<GetMirrorUserDto> response = new();

        var user = await _mirrorDbContext.MirrorUsers
            .Where(x =>
                x.UserId == request.UserId &&
                x.Status == (int)EventStatus.Complete
            )
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (user == null)
        {
            response.StatusCode = HttpStatusCode.NotFound;
            response.Message = "User not found";

            return response;
        }
        else
        {
            response.StatusCode = HttpStatusCode.OK;
            response.Message = "User found";
            response.Result = new GetMirrorUserDto
            {
                UserId = request.UserId,
                HasSubscription = user.ExpireDate > DateTime.UtcNow,
                SubscriptionUntil = user.ExpireDate,
                MemberSince = user.CreatedDate
            };

            return response;
        }
    }
}