using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoFramework.Linq;
using Newtonsoft.Json;

namespace ZiziBot.Application.Handlers.RestApis.DashboardSession;

public class SaveTelegramSessionRequest : ApiRequestBase<SaveDashboardSessionIdResponseDto>
{
    [FromBody]
    public SaveTelegramSessionRequestModel Model { get; set; }
}

public class SaveTelegramSessionRequestModel
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("first_name")]
    public string FirstName { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("photo_url")]
    public string PhotoUrl { get; set; }

    [JsonProperty("auth_date")]
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

public class SaveTelegramSessionRequestHandler : IRequestHandler<SaveTelegramSessionRequest, ApiResponseBase<SaveDashboardSessionIdResponseDto>>
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

    public async Task<ApiResponseBase<SaveDashboardSessionIdResponseDto>> Handle(SaveTelegramSessionRequest request, CancellationToken cancellationToken)
    {
        ApiResponseBase<SaveDashboardSessionIdResponseDto> response = new();

        var botSetting = await _appSettingsDbContext.BotSettings
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .Where(entity => entity.Name == "Main")
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (botSetting == null)
        {
            return response.BadRequest("Suatu kesalahan terjadi, silahkan hubungi admin");
        }

        LoginWidget loginWidget = new(botSetting.Token);

        var sessionData = request.Model.ToDictionary()
            .Where(pair => pair.Key != "session_id");

        var checkAuthorization = loginWidget.CheckAuthorization(sessionData);
        if (checkAuthorization != WebAuthorization.Valid)
        {
            return response.Unauthorized($"Sesi tidak valid, silakan kirim ulang perintah '/console' di Bot untuk membuat sesi baru.");
        }

        var checkSudo = await _appSettingsDbContext.Sudoers
            .Where(entity => entity.UserId == request.Model.Id)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, request.Model.Username),
            new Claim(ClaimTypes.Name, request.Model.FirstName),
            new Claim(ClaimTypes.Role, checkSudo == null ? "User" : "Sudoer"),
            new Claim(HeaderKey.UserId, request.Model.Id.ToString()),
            new Claim("photoUrl", request.Model.PhotoUrl),
        };

        var token = new JwtSecurityToken(JwtConfig.Issuer, JwtConfig.Audience, claims, expires: DateTime.Now.AddMinutes(15), signingCredentials: credentials);
        var stringToken = new JwtSecurityTokenHandler().WriteToken(token);

        _userDbContext.DashboardSessions.Add(new DashboardSessionEntity()
        {
            TelegramUserId = request.Model.Id,
            FirstName = request.Model.FirstName,
            PhotoUrl = request.Model.PhotoUrl,
            Username = request.Model.Username,
            AuthDate = request.Model.AuthDate,
            Hash = request.Model.Hash,
            SessionId = request.Model.SessionId,
            BearerToken = stringToken,
            Status = (int)EventStatus.Complete
        });

        await _userDbContext.SaveChangesAsync(cancellationToken);

        return response.Success("Session saved successfully", new SaveDashboardSessionIdResponseDto()
        {
            IsSessionValid = true,
            BearerToken = stringToken
        });
    }
}