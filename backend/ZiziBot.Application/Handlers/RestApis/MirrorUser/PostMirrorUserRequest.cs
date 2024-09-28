using MongoFramework.Linq;
using ZiziBot.DataSource.MongoDb.Entities;

namespace ZiziBot.Application.Handlers.RestApis.MirrorUser;

public class PostMirrorUserRequestDto : ApiRequestBase<bool>
{
    public long UserId { get; set; }
    public int AddDays { get; set; }
    public int MonthDuration { get; set; }
    public string? AdditionalNote { get; set; }
}

public class PostMirrorUserRequestHandler(MongoDbContextBase mongoDbContext) : IApiRequestHandler<PostMirrorUserRequestDto, bool>
{
    private readonly ApiResponseBase<bool> _response = new();

    public async Task<ApiResponseBase<bool>> Handle(PostMirrorUserRequestDto request,
        CancellationToken cancellationToken)
    {
        var mirrorUser = await mongoDbContext.MirrorUsers
            .FirstOrDefaultAsync(entity =>
                    entity.UserId == request.UserId &&
                    entity.Status == (int)EventStatus.Complete,
                cancellationToken: cancellationToken);

        if (mirrorUser == null)
        {
            mongoDbContext.MirrorUsers.Add(new MirrorUserEntity() {
                UserId = request.UserId,
                ExpireDate = DateTime.UtcNow.AddDays(request.AddDays),
                Status = (int)EventStatus.Complete
            });
        }
        else
        {
            mirrorUser.ExpireDate = mirrorUser.ExpireDate.AddMonths(request.MonthDuration);
        }

        await mongoDbContext.SaveChangesAsync(cancellationToken);

        return _response.Success("Mirror User saved", true);
    }
}