using MediatR;

namespace ZiziBot.Application.Pipelines;

public class PostMirrorUserRequestDto : IRequest<bool>
{
    public long UserId { get; set; }
    public int AddDays { get; set; }
}

public class PostMirrorUserRequestHandler : IRequestHandler<PostMirrorUserRequestDto, bool>
{
    private readonly MirrorDbContext _mirrorDbContext;

    public PostMirrorUserRequestHandler(MirrorDbContext mirrorDbContext)
    {
        _mirrorDbContext = mirrorDbContext;
    }

    public async Task<bool> Handle(PostMirrorUserRequestDto request, CancellationToken cancellationToken)
    {
        _mirrorDbContext.MirrorUsers.Add(new MirrorUser()
        {
            UserId = request.UserId,
            ExpireAt = DateTime.UtcNow.AddDays(request.AddDays),
            Status = (int)EventStatus.Complete
        });

        await _mirrorDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}