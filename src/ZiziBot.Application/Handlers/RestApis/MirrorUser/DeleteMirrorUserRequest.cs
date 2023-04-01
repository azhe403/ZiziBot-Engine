using System.Net;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.MirrorUser;

public class DeleteMirrorUserRequestDto : ApiRequestBase<bool>
{
    public long UserId { get; set; }
}

public class DeleteMirrorUserRequestHandler : IRequestHandler<DeleteMirrorUserRequestDto, ApiResponseBase<bool>>
{
    private readonly MirrorDbContext _mirrorDbContext;

    public DeleteMirrorUserRequestHandler(MirrorDbContext mirrorDbContext)
    {
        _mirrorDbContext = mirrorDbContext;
    }

    public async Task<ApiResponseBase<bool>> Handle(DeleteMirrorUserRequestDto request, CancellationToken cancellationToken)
    {
        var mirrorUser = await _mirrorDbContext.MirrorUsers
            .FirstOrDefaultAsync(user => user.UserId == request.UserId, cancellationToken: cancellationToken);

        mirrorUser.Status = (int)EventStatus.Deleted;

        await _mirrorDbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponseBase<bool>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "User saved",
            Result = true
        };
    }
}