using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.MirrorUser;

public class DeleteMirrorUserRequestDto : IRequest<bool>
{
    public long UserId { get; set; }
}

public class DeleteMirrorUserRequestHandler : IRequestHandler<DeleteMirrorUserRequestDto, bool>
{
    private readonly MirrorDbContext _mirrorDbContext;

    public DeleteMirrorUserRequestHandler(MirrorDbContext mirrorDbContext)
    {
        _mirrorDbContext = mirrorDbContext;
    }

    public async Task<bool> Handle(DeleteMirrorUserRequestDto request, CancellationToken cancellationToken)
    {
        var mirrorUser = await _mirrorDbContext.MirrorUsers
            .FirstOrDefaultAsync(user => user.UserId == request.UserId, cancellationToken: cancellationToken);

        mirrorUser.Status = (int) EventStatus.Deleted;

        await _mirrorDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}