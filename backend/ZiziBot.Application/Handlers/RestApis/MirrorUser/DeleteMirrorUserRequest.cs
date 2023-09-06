using System.Net;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.MirrorUser;

public class DeleteMirrorUserRequestDto : ApiRequestBase<bool>
{
    public long UserId { get; set; }
}

public class DeleteMirrorUserRequestHandler : IRequestHandler<DeleteMirrorUserRequestDto, ApiResponseBase<bool>>
{
    private readonly MongoDbContextBase _mongoDbContext;

    public DeleteMirrorUserRequestHandler(MongoDbContextBase mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }

    public async Task<ApiResponseBase<bool>> Handle(DeleteMirrorUserRequestDto request, CancellationToken cancellationToken)
    {
        var mirrorUser = await _mongoDbContext.MirrorUsers
            .FirstOrDefaultAsync(user => user.UserId == request.UserId, cancellationToken: cancellationToken);

        mirrorUser.Status = (int)EventStatus.Deleted;

        await _mongoDbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponseBase<bool>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "User saved",
            Result = true
        };
    }
}