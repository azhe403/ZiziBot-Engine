using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.Application.Handlers.RestApis.DashboardSession;

public class ValidateTelegramSessionRequest : ApiPostRequestBase<TelegramSessionDto, ValidateDashboardSessionIdResponseDto>
{ }

public class ValidateDashboardSessionIdResponseDto
{
    public bool IsSessionValid { get; set; }
    public string BearerToken { get; set; }
}

public class ValidateTelegramSessionHandler(
    ILogger<ValidateTelegramSessionHandler> logger,
    DataFacade dataFacade
)
    : IApiRequestHandler<ValidateTelegramSessionRequest, ValidateDashboardSessionIdResponseDto>
{
    public async Task<ApiResponseBase<ValidateDashboardSessionIdResponseDto>> Handle(
        ValidateTelegramSessionRequest request,
        CancellationToken cancellationToken
    )
    {
        ApiResponseBase<ValidateDashboardSessionIdResponseDto> response = new();

        var botSetting = await dataFacade.MongoEf.BotSettings
            .Where(entity => entity.Status == EventStatus.Complete)
            .Where(entity => entity.Name == "Main")
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (botSetting == null)
        {
            return response.BadRequest("Suatu kesalahan terjadi, silahkan hubungi admin");
        }

        LoginWidget loginWidget = new(botSetting.Token);

        var sessionData = request.Body.ToDictionary()
            .Select(pair => new KeyValuePair<string, string>(pair.Key.Replace(" ", "_"), pair.Value))
            .Where(pair => pair.Value.IsNotNullOrEmpty())
            .Where(pair => pair.Key != "session_id");

        var checkAuthorization = loginWidget.CheckAuthorization(sessionData);
        if (checkAuthorization != WebAuthorization.Valid)
        {
            logger.LogDebug("Session is not valid for SessionId: {SessionId}. Result: {Result}",
                request.Body.SessionId, checkAuthorization);

            return response.Unauthorized(
                $"Sesi tidak valid, silakan kirim ulang perintah '/console' di Bot untuk membuat sesi baru.");
        }

        var jwtConfig = await dataFacade.AppSetting.GetConfigSectionAsync<JwtConfig>();

        if (jwtConfig == null)
        {
            return response.BadRequest("Authentication is not yet configured");
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, request.Body.Username),
            new Claim(ClaimTypes.Name, request.Body.FirstName),
            new Claim(ClaimTypes.Role, request.SessionUserRole.ToString()),
            new Claim(RequestKey.UserId, request.Body.Id.ToString()),
            new Claim("photoUrl", request.Body.PhotoUrl ?? ""),
        };

        var token = new JwtSecurityToken(jwtConfig.Issuer, jwtConfig.Audience, claims,
            expires: DateTime.Now.AddMinutes(15), signingCredentials: credentials);

        var stringToken = new JwtSecurityTokenHandler().WriteToken(token);

        dataFacade.MongoEf.DashboardSessions.Add(new DashboardSessionEntity() {
            TelegramUserId = request.Body.Id,
            FirstName = request.Body.FirstName,
            LastName = request.Body.LastName,
            PhotoUrl = request.Body.PhotoUrl,
            Username = request.Body.Username,
            AuthDate = request.Body.AuthDate,
            Hash = request.Body.Hash,
            SessionId = request.Body.SessionId,
            BearerToken = stringToken,
            Status = EventStatus.Complete
        });

        await dataFacade.MongoEf.SaveChangesAsync(cancellationToken);

        return response.Success("Session saved successfully", new ValidateDashboardSessionIdResponseDto() {
            IsSessionValid = true,
            BearerToken = stringToken
        });
    }
}