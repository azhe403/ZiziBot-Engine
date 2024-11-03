using Microsoft.EntityFrameworkCore;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.Application.Handlers.RestApis.MirrorUser;

public class PostMirrorUserRequestDto : ApiRequestBase<bool>
{
    public long UserId { get; set; }
    public int AddDays { get; set; }
    public int MonthDuration { get; set; }
    public string? AdditionalNote { get; set; }
}

public class PostMirrorUserRequestHandler(
    DataFacade dataFacade
) : IApiRequestHandler<PostMirrorUserRequestDto, bool>
{
    private readonly ApiResponseBase<bool> _response = new();

    public async Task<ApiResponseBase<bool>> Handle(PostMirrorUserRequestDto request, CancellationToken cancellationToken)
    {
        var mirrorUser = await dataFacade.MongoEf.MirrorUser
            .Where(entity => entity.UserId == request.UserId)
            .Where(entity => entity.Status == EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (mirrorUser == null)
        {
            dataFacade.MongoEf.MirrorUser.Add(new MirrorUserEntity() {
                UserId = request.UserId,
                ExpireDate = DateTime.UtcNow.AddDays(request.AddDays),
                Status = EventStatus.Complete
            });
        }
        else
        {
            mirrorUser.ExpireDate = mirrorUser.ExpireDate.AddMonths(request.MonthDuration);
        }

        await dataFacade.MongoEf.SaveChangesAsync(cancellationToken);

        return _response.Success("Mirror User saved", true);
    }
}