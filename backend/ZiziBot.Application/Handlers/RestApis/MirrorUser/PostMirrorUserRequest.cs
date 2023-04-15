using System.Net;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.MirrorUser;

public class PostMirrorUserRequestDto : ApiRequestBase<bool>
{
    public long UserId { get; set; }
    public int AddDays { get; set; }
    public int MonthDuration { get; set; }
    public string? AdditionalNote { get; set; }
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
        var mirrorUser = await _mirrorDbContext.MirrorUsers
            .FirstOrDefaultAsync(entity =>
                    entity.UserId == request.UserId &&
                    entity.Status == (int) EventStatus.Complete,
                cancellationToken: cancellationToken);

        if (mirrorUser == null)
        {
            _mirrorDbContext.MirrorUsers.Add(new MirrorUserEntity()
            {
                UserId = request.UserId,
                ExpireDate = DateTime.UtcNow.AddDays(request.AddDays),
                Status = (int) EventStatus.Complete
            });
        }
        else
        {
            mirrorUser.ExpireDate = mirrorUser.ExpireDate.AddMonths(request.MonthDuration);
        }

        await _mirrorDbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponseBase<bool>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "User saved",
            Result = true
        };
    }
}