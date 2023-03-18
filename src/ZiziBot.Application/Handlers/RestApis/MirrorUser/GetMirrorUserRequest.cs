using System.Net;
using MongoDB.Bson;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.MirrorUser;

public class GetMirrorUsersRequestDto : ApiRequestBase<IEnumerable<GetMirrorUserResponseDto>>
{
}

public class GetMirrorUserResponseDto
{
    public ObjectId Id { get; set; }
    public long UserId { get; set; }
    public DateTime ExpireDate { get; set; }
    public int Status { get; set; }
    public string TransactionId { get; set; }
    public DateTime MemberSince { get; set; }
    public DateTime LastUpdate { get; set; }
}

public class GetMirrorUsersRequestHandler : IRequestHandler<GetMirrorUsersRequestDto, ApiResponseBase<IEnumerable<GetMirrorUserResponseDto>>>
{
    private readonly MirrorDbContext _mirrorDbContext;

    public GetMirrorUsersRequestHandler(MirrorDbContext mirrorDbContext)
    {
        _mirrorDbContext = mirrorDbContext;
    }

    public async Task<ApiResponseBase<IEnumerable<GetMirrorUserResponseDto>>> Handle(GetMirrorUsersRequestDto request, CancellationToken cancellationToken)
    {
        var user = await _mirrorDbContext.MirrorUsers
            .Where(mirrorUser => mirrorUser.Status == (int) EventStatus.Complete)
            .ToListAsync(cancellationToken: cancellationToken);

        var mirrorUsers = user.Select(mirrorUser => new GetMirrorUserResponseDto
        {
            Id = mirrorUser.Id,
            UserId = mirrorUser.UserId,
            ExpireDate = mirrorUser.ExpireDate,
            Status = mirrorUser.Status,
            TransactionId = mirrorUser.TransactionId,
            MemberSince = mirrorUser.CreatedDate,
            LastUpdate = mirrorUser.UpdatedDate
        });

        return new ApiResponseBase<IEnumerable<GetMirrorUserResponseDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Users found",
            Result = mirrorUsers
        };
    }
}