using System.ComponentModel;

namespace ZiziBot.Common.Configs;

[DisplayName("Sentry")]
public class SentryConfig
{
    public bool IsEnabled { get; set; }
    public string Dsn { get; set; }
}