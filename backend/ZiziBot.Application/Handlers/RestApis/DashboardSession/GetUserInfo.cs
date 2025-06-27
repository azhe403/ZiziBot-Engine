namespace ZiziBot.Application.Handlers.RestApis.DashboardSession;

public class GetUserInfoRequest : ApiRequestBase<GetUserInfoResponse>
{ }

public class GetUserInfoResponse
{
    public bool IsSessionValid { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public long UserId { get; set; }
    public string? PhotoUrl { get; set; }
    public List<RoleLevel> Roles { get; set; }
}

public class GetUserInfoHandler(DataFacade dataFacade) : IApiRequestHandler<GetUserInfoRequest, GetUserInfoResponse>
{
    public async Task<ApiResponseBase<GetUserInfoResponse>> Handle(GetUserInfoRequest request, CancellationToken cancellationToken)
    {
        var response = ApiResponse.Create<GetUserInfoResponse>();

        await Task.Delay(1, cancellationToken);

        if (!request.UserInfo.IsAuthenticated)
            return response.BadRequest("Session is not valid");

        var getUserInfoResponse = new GetUserInfoResponse() {
            IsSessionValid = request.UserInfo.IsAuthenticated,
            UserName = request.UserInfo.UserName,
            UserId = request.UserInfo.UserId,
            FirstName = request.UserInfo.UserFirstName,
            LastName = request.UserInfo.UserLastName,
            PhotoUrl = request.UserInfo.UserPhotoUrl,
            Roles = request.UserInfo.UserRoles
        };

        return response.Success("Get User Info success", getUserInfoResponse);
    }
}