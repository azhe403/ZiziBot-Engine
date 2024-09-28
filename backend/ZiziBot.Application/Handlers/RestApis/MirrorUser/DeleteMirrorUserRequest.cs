using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.MirrorUser;

public class DeleteMirrorUserRequestDto : ApiRequestBase<bool>
{
    public long UserId { get; set; }
}

public class DeleteMirrorUserRequestHandler(MongoDbContextBase mongoDbContext) : IApiRequestHandler<DeleteMirrorUserRequestDto, bool>
{
    private readonly ApiResponseBase<bool> _response = new();

    public async Task<ApiResponseBase<bool>> Handle(DeleteMirrorUserRequestDto request, CancellationToken cancellationToken)
    {
        var mirrorUser = await mongoDbContext.MirrorUsers
            .Where(x => x.Status == (int)EventStatus.Complete)
            .Where(x => x.UserId == request.UserId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (mirrorUser == null)
        {
            return _response.BadRequest("Mirror User not found");
        }

        mirrorUser.Status = (int)EventStatus.Deleted;

        await mongoDbContext.SaveChangesAsync(cancellationToken);

        return _response.Success("Mirror User deleted", true);
    }
}