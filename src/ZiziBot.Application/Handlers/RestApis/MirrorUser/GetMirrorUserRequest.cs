using System.Net;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.MirrorUser;

public class GetMirrorUsersRequestDto : ApiRequestBase<List<MirrorUserEntity>>
{
}

public class GetMirrorUsersRequestHandler : IRequestHandler<GetMirrorUsersRequestDto, ApiResponseBase<List<MirrorUserEntity>>>
{
    private readonly MirrorDbContext _mirrorDbContext;

    public GetMirrorUsersRequestHandler(MirrorDbContext mirrorDbContext)
    {
        _mirrorDbContext = mirrorDbContext;
    }

    public async Task<ApiResponseBase<List<MirrorUserEntity>>> Handle(GetMirrorUsersRequestDto request, CancellationToken cancellationToken)
    {
        var user = await _mirrorDbContext.MirrorUsers
            .Where(mirrorUser => mirrorUser.Status == (int) EventStatus.Complete)
            .ToListAsync(cancellationToken: cancellationToken);

        return new ApiResponseBase<List<MirrorUserEntity>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Users found",
            Result = user
        };
    }
}