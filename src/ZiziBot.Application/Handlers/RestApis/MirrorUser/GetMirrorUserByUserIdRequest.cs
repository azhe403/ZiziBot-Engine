using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.MirrorUser;

public class GetMirrorUserByUserIdRequestDto : IRequest<MirrorUserEntity>
{
    public long UserId { get; set; }
}

public class GetMirrorUserByUserIdRequestHandler : IRequestHandler<GetMirrorUserByUserIdRequestDto, MirrorUserEntity>
{
    private readonly MirrorDbContext _mirrorDbContext;

    public GetMirrorUserByUserIdRequestHandler(MirrorDbContext mirrorDbContext)
    {
        _mirrorDbContext = mirrorDbContext;
    }

    public async Task<MirrorUserEntity> Handle(GetMirrorUserByUserIdRequestDto request, CancellationToken cancellationToken)
    {
        var user = await _mirrorDbContext.MirrorUsers
            .Where(x => x.UserId == request.UserId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return user;
    }
}