using System.Net;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.GlobalBan;

public class GetGlobalBanApiRequest : ApiRequestBase<List<GetGlobalBanApiDto>>
{
}

public class GetGlobalBanApiDto
{
    public long UserId { get; set; }
    public long ChatId { get; set; }
    public string Reason { get; set; }
    public int Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime DeletedDate { get; set; }
}

public class GetGlobalBanApiRequestHandler : IRequestHandler<GetGlobalBanApiRequest, ApiResponseBase<List<GetGlobalBanApiDto>>>
{
    private readonly AntiSpamDbContext _antiSpamDbContext;

    public GetGlobalBanApiRequestHandler(AntiSpamDbContext antiSpamDbContext)
    {
        _antiSpamDbContext = antiSpamDbContext;
    }

    public async Task<ApiResponseBase<List<GetGlobalBanApiDto>>> Handle(GetGlobalBanApiRequest request, CancellationToken cancellationToken)
    {
        var data = await _antiSpamDbContext.GlobalBan
            .ToListAsync(cancellationToken: cancellationToken);

        var globalBanList = data.Select(
            x => new GetGlobalBanApiDto()
            {
                UserId = x.UserId,
                ChatId = x.ChatId,
                Reason = x.Reason,
                Status = x.Status,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
                DeletedDate = x.DeletedDate
            }
        ).ToList();

        return new ApiResponseBase<List<GetGlobalBanApiDto>>()
        {
            Result = globalBanList,
            StatusCode = HttpStatusCode.OK
        };
    }
}