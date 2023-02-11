using System.Net;

namespace ZiziBot.Application.Handlers.RestApis.MirrorUser;

public class PostMirrorUserRequestDto : ApiRequestBase<bool>
{
    public long UserId { get; set; }
    public int AddDays { get; set; }
}

public class PostMirrorUserRequestHandler : IRequestHandler<PostMirrorUserRequestDto, ApiResponseBase<bool>>
{
    private readonly MirrorDbContext _mirrorDbContext;

    public PostMirrorUserRequestHandler(MirrorDbContext mirrorDbContext)
    {
        _mirrorDbContext = mirrorDbContext;
    }

    public async Task<ApiResponseBase<bool>> Handle(PostMirrorUserRequestDto request, CancellationToken cancellationToken)
    {
        _mirrorDbContext.MirrorUsers.Add(
            new MirrorUserEntity()
            {
                UserId = request.UserId,
                ExpireDate = DateTime.UtcNow.AddDays(request.AddDays),
                Status = (int) EventStatus.Complete
            }
        );

        await _mirrorDbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponseBase<bool>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "User added",
            Result = true
        };
    }
}