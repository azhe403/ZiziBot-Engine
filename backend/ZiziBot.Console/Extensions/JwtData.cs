using System.IdentityModel.Tokens.Jwt;

namespace ZiziBot.Console.Extensions;

public class JwtData
{
    public string Aud { get; set; }
    public string Iss { get; set; }
    public long Iat { get; set; }
    public long Nbf { get; set; }
    public long Exp { get; set; }
    public string Aio { get; set; }
    public string Azp { get; set; }
    public long Azpacr { get; set; }
    public string Name { get; set; }
    public string Oid { get; set; }
    public string PreferredUsername { get; set; }
    public string Rh { get; set; }
    public List<string> Roles { get; set; }
    public string Scp { get; set; }
    public string Sub { get; set; }
    public string Sid { get; set; }
    public string Tid { get; set; }
    public string Uti { get; set; }
    public string Ver { get; set; }
    public JwtPayload Data { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
}