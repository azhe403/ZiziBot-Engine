using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ZiziBot.Common.Exceptions;

namespace ZiziBot.Application.UseCases.User;

public class GenerateAccessTokenResult
{
    public required string AccessToken { get; set; }
    public double AccessExpireIn { get; set; }
    public DateTime AccessExpireDate { get; set; }
}

public class GenerateAccessTokenUseCase(AppSettingRepository appSettingRepository)
{
    public async Task<GenerateAccessTokenResult> Handle(long userId)
    {
        var jwtConfig = await appSettingRepository.GetConfigSectionAsync<JwtConfig>();

        if (jwtConfig == null)
            throw new AppException("JWT Config not found");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[] {
            new Claim(RequestKey.UserId, userId.ToString()),
        };

        var dateTime = DateTime.UtcNow;
        var tokenExpiration = dateTime.AddMinutes(15);
        var accessExpireIn = (tokenExpiration - dateTime).TotalSeconds;
        var token = new JwtSecurityToken(issuer: jwtConfig.Issuer,
            audience: jwtConfig.Audience,
            claims: claims,
            expires: tokenExpiration,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new GenerateAccessTokenResult {
            AccessToken = accessToken,
            AccessExpireIn = accessExpireIn,
            AccessExpireDate = tokenExpiration
        };
    }
}