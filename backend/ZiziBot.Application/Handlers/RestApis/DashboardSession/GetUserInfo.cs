using Microsoft.EntityFrameworkCore;

namespace ZiziBot.Application.Handlers.RestApis.DashboardSession;

public class GetUserInfoRequest : ApiRequestBase<GetUserInfoResponse>
{ }

public class GetUserInfoResponse
{
    public bool IsSessionValid { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public long UserId { get; set; }
    public string PhotoUrl { get; set; }
}

public class GetUserInfoHandler(DataFacade dataFacade) : IApiRequestHandler<GetUserInfoRequest, GetUserInfoResponse>
{
    private ApiResponseBase<GetUserInfoResponse> response = new();

    public async Task<ApiResponseBase<GetUserInfoResponse>> Handle(GetUserInfoRequest request, CancellationToken cancellationToken)
    {
        var session = await dataFacade.MongoEf.DashboardSessions
            .Where(x => x.BearerToken == request.BearerToken)
            .Where(x => x.Status == EventStatus.Complete)
            .OrderByDescending(x => x.CreatedDate)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (session == null)
        {
            return response.Unauthorized("Sesi tidak valid, silakan login kembali", new GetUserInfoResponse() {
                IsSessionValid = false
            });
        }

        return response.Success("Get User Info success", new GetUserInfoResponse() {
            IsSessionValid = true,
            UserName = session.Username,
            UserId = session.TelegramUserId,
            Name = (session.FirstName + " " + session.LastName).Trim(),
            PhotoUrl = session.PhotoUrl,
        });
    }
}