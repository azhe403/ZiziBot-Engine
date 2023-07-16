using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace ZiziBot.Console.Extensions;

public static class TokenUtil
{
    public static JwtSecurityToken DecodeJwtToken(this string bearerToken)
    {
        var token = bearerToken.Replace("Bearer ", "");
        var decodedToken = new JwtSecurityToken(token);
        return decodedToken;
    }

    public static string? GetClaimValue(this JwtSecurityToken token, string claimType)
    {
        var claim = token.Claims.FirstOrDefault(c => c.Type == claimType);
        return claim?.Value;
    }
}