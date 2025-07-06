using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ZiziBot.Common.Exceptions;
using ZiziBot.Common.Interfaces;
using ZiziBot.Database.MongoDb;

namespace ZiziBot.Services;

public class UserService(MongoDbContext mongoDbContext, AppSettingRepository appSettingRepository) : IUserService
{
    public async Task<(string stringToken, DateTime tokenExpiration, int accessExpireIn)> GenerateAccessToken(long userId)
    {
        var jwtConfig = await appSettingRepository.GetConfigSectionAsync<JwtConfig>();

        if (jwtConfig == null)
            throw new AppException("JWT Config not found");

        var botUser = await mongoDbContext.BotUser.Where(x => x.UserId == userId)
            .FirstOrDefaultAsync();

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, botUser?.Username ?? string.Empty),
            new Claim(ClaimTypes.Name, botUser?.FirstName ?? string.Empty),
            new Claim(RequestKey.UserId, userId.ToString()),
        };

        var dateTime = DateTime.UtcNow;
        var tokenExpiration = dateTime.AddMinutes(15);
        var accessExpireIn = (tokenExpiration - dateTime).TotalSeconds;
        var token = new JwtSecurityToken(jwtConfig.Issuer, jwtConfig.Audience, claims, expires: tokenExpiration, signingCredentials: credentials);
        var stringToken = new JwtSecurityTokenHandler().WriteToken(token);

        return (stringToken, tokenExpiration, (int)accessExpireIn);
    }
}