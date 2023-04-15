namespace ZiziBot.Application.Handlers.RestApis.Core;

public class GetWelcomeRequest : ApiRequestBase<GetWelcomeResponse>
{
}

public class GetWelcomeResponse
{
    public required string AppName { get; set; }
    public required string AppVersion { get; set; }
    public required string VersionNumber { get; set; }
    public required string BuildDate { get; set; }
}

public class GetWelcomeHandler : IRequestHandler<GetWelcomeRequest, ApiResponseBase<GetWelcomeResponse>>
{
    public async Task<ApiResponseBase<GetWelcomeResponse>> Handle(GetWelcomeRequest request, CancellationToken cancellationToken)
    {
        var response = new ApiResponseBase<GetWelcomeResponse>();

        return response.Success("Welcome to ZiziBot", new GetWelcomeResponse
        {
            AppName = "ZiziBot 5",
            AppVersion = VersionUtil.GetVersion(true),
            VersionNumber = VersionUtil.GetVersion(),
            BuildDate = VersionUtil.GetBuildDate().ToString("yyyy-MM-dd HH:mm:ss zzz")
        });
    }
}