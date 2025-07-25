using System.ComponentModel;

namespace ZiziBot.Common.Configs;

[DisplayName("Jwt")]
public class JwtConfig
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}