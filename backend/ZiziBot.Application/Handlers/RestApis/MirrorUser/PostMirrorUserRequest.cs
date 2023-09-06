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
    private readonly MongoDbContextBase _mongoDbContext;

    public PostMirrorUserRequestHandler(MongoDbContextBase mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }

    public async Task<ApiResponseBase<bool>> Handle(PostMirrorUserRequestDto request, CancellationToken cancellationToken)
    {
        var mirrorUser = await _mongoDbContext.MirrorUsers
            .FirstOrDefaultAsync(entity =>
                    entity.UserId == request.UserId &&
                    entity.Status == (int)EventStatus.Complete,
                cancellationToken: cancellationToken);

        if (mirrorUser == null)
        {
            _mongoDbContext.MirrorUsers.Add(new MirrorUserEntity()
            {
                UserId = request.UserId,
                ExpireDate = DateTime.UtcNow.AddDays(request.AddDays),
                Status = (int)EventStatus.Complete
            });
        }
        else
        {
            mirrorUser.ExpireDate = mirrorUser.ExpireDate.AddMonths(request.MonthDuration);
        }

        await _mongoDbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponseBase<bool>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "User saved",
            Result = true
        };
    }
}