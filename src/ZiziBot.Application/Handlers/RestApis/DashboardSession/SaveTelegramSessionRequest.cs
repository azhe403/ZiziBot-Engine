using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoFramework.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ZiziBot.Application.Handlers.RestApis.DashboardSession;

public class SaveTelegramSessionRequestModel : ApiRequestBase<SaveDashboardSessionIdResponseDto>
{
    [JsonProperty("id")]
    public long TelegramUserId { get; set; }

    [JsonProperty("first_name")]
    public string FirstName { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("photo_url")]
    public string PhotoUrl { get; set; }

    [JsonConverter(typeof(UnixDateTimeConverter))]
    public long AuthDate { get; set; }

    [JsonProperty("hash")]
    public string Hash { get; set; }

    [JsonProperty("session_id")]
    public string SessionId { get; set; }
}

public class SaveDashboardSessionIdResponseDto
{
    public bool IsSessionValid { get; set; }
    public string BearerToken { get; set; }
}

public class SaveTelegramSessionRequestHandler : IRequestHandler<SaveTelegramSessionRequestModel, ApiResponseBase<SaveDashboardSessionIdResponseDto>>
{
    private readonly ILogger<SaveTelegramSessionRequestHandler> _logger;
    private readonly IOptions<JwtConfig> _jwtConfigOption;
    private readonly AppSettingsDbContext _appSettingsDbContext;
    private readonly UserDbContext _userDbContext;

    private JwtConfig JwtConfig => _jwtConfigOption.Value;

    public SaveTelegramSessionRequestHandler(
        ILogger<SaveTelegramSessionRequestHandler> logger,
        IOptions<JwtConfig> jwtConfigOption,
        AppSettingsDbContext appSettingsDbContext,
        UserDbContext userDbContext
    )
    {
        _logger = logger;
        _jwtConfigOption = jwtConfigOption;
        _appSettingsDbContext = appSettingsDbContext;
        _userDbContext = userDbContext;
    }

    public async Task<ApiResponseBase<SaveDashboardSessionIdResponseDto>> Handle(SaveTelegramSessionRequestModel request, CancellationToken cancellationToken)
    {
        ApiResponseBase<SaveDashboardSessionIdResponseDto> apiResponse = new();

        var checkSudo = await _appSettingsDbContext.Sudoers
            .FirstOrDefaultAsync(sudoer =>
                    sudoer.UserId == request.TelegramUserId &&
                    sudoer.Status == (int)EventStatus.Complete,
                cancellationToken: cancellationToken
            );

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, request.Username),
            new Claim(ClaimTypes.Name, request.FirstName),
            new Claim(ClaimTypes.Role, checkSudo == null ? "User" : "Sudoer"),
            new Claim(HeaderKey.UserId, request.TelegramUserId.ToString()),
            new Claim("photoUrl", request.PhotoUrl),
        };
        var token = new JwtSecurityToken(JwtConfig.Issuer, JwtConfig.Audience, claims, expires: DateTime.Now.AddMinutes(15), signingCredentials: credentials);


        var stringToken = new JwtSecurityTokenHandler().WriteToken(token);

        var dashboardSession = await _userDbContext.DashboardSessions
            .FirstOrDefaultAsync(
                session =>
                    session.TelegramUserId == request.TelegramUserId &&
                    session.Status == (int)EventStatus.Complete,
                cancellationToken: cancellationToken
            );

        if (dashboardSession == null)
        {
            _logger.LogDebug("New dashboard session created for user {UserId}", request.TelegramUserId);

            _userDbContext.DashboardSessions.Add(
                new DataSource.MongoDb.Entities.DashboardSession()
                {
                    TelegramUserId = request.TelegramUserId,
                    FirstName = request.FirstName,
                    PhotoUrl = request.PhotoUrl,
                    Username = request.Username,
                    AuthDate = request.AuthDate,
                    Hash = request.Hash,
                    SessionId = request.SessionId,
                    BearerToken = stringToken,
                    Status = (int)EventStatus.Complete
                }
            );
        }
        else
        {
            _logger.LogDebug("Dashboard session updated for user {UserId}", request.TelegramUserId);

            dashboardSession.FirstName = request.FirstName;
            dashboardSession.PhotoUrl = request.PhotoUrl;
            dashboardSession.Username = request.Username;
            dashboardSession.AuthDate = request.AuthDate;
            dashboardSession.Hash = request.Hash;
            dashboardSession.SessionId = request.SessionId;
            dashboardSession.BearerToken = stringToken;
            dashboardSession.Status = (int)EventStatus.Complete;
        }

        await _userDbContext.SaveChangesAsync(cancellationToken);

        return apiResponse.Success("Session saved successfully", new SaveDashboardSessionIdResponseDto()
        {
            IsSessionValid = true,
            BearerToken = stringToken
        });
    }
}