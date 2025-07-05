using System.ComponentModel;

namespace ZiziBot.Common.Configs;

[DisplayName("Gcp")]
public class GcpConfig
{
    public bool IsEnabled { get; set; }
    public string FirebaseProjectUrl { get; set; }
    public string FirebaseServiceAccountJson { get; set; }
}