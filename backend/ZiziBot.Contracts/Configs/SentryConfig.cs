using System.ComponentModel;

namespace ZiziBot.Contracts.Configs;

[DisplayName("Sentry")]
public class SentryConfig
{
    public bool IsEnabled { get; set; }
    public string Dsn { get; set; }
}