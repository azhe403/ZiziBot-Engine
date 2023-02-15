using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoFramework.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ZiziBot.Application.Handlers.RestApis.DashboardSession;

public class SaveTelegramSessionRequestModel : IRequest<bool>
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

public class SaveTelegramSessionRequestHandler : IRequestHandler<SaveTelegramSessionRequestModel, bool>
{
    private readonly ILogger<SaveTelegramSessionRequestHandler> _logger;
    private readonly AppSettingsDbContext _appSettingsDbContext;
    private readonly UserDbContext _userDbContext;

    public SaveTelegramSessionRequestHandler(ILogger<SaveTelegramSessionRequestHandler> logger, AppSettingsDbContext appSettingsDbContext, UserDbContext userDbContext)
    {
        _logger = logger;
        _appSettingsDbContext = appSettingsDbContext;
        _userDbContext = userDbContext;
    }

    public async Task<bool> Handle(SaveTelegramSessionRequestModel request, CancellationToken cancellationToken)
    {
        var checkSudo = await _appSettingsDbContext.Sudoers
            .FirstOrDefaultAsync(
                sudoer =>
                    sudoer.UserId == request.TelegramUserId &&
                    sudoer.Status == (int) EventStatus.Complete,
                cancellationToken: cancellationToken
            );

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(
                new[]
                {
                    new Claim("userId", request.TelegramUserId.ToString()),
                    new Claim(ClaimTypes.Role, checkSudo == null ? "User" : "Sudoer")
                }
            ),
            Expires = DateTime.UtcNow.AddDays(1)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var stringToken = tokenHandler.WriteToken(token);

        var dashboardSession = await _userDbContext.DashboardSessions
            .FirstOrDefaultAsync(
                session =>
                    session.TelegramUserId == request.TelegramUserId &&
                    session.Status == (int) EventStatus.Complete,
                cancellationToken: cancellationToken
            );

        if (dashboardSession == null)
        {
            _logger.LogInformation("New dashboard session created for user {0}", request.TelegramUserId);

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
                    Status = (int) EventStatus.Complete
                }
            );
        }
        else
        {
            _logger.LogInformation("Dashboard session updated for user {0}", request.TelegramUserId);

            dashboardSession.FirstName = request.FirstName;
            dashboardSession.PhotoUrl = request.PhotoUrl;
            dashboardSession.Username = request.Username;
            dashboardSession.AuthDate = request.AuthDate;
            dashboardSession.Hash = request.Hash;
            dashboardSession.SessionId = request.SessionId;
            dashboardSession.BearerToken = stringToken;
            dashboardSession.Status = (int) EventStatus.Complete;
        }


        await _userDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}