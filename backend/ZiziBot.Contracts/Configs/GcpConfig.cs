using System.ComponentModel;

namespace ZiziBot.Contracts.Configs;

[DisplayName("Gcp")]
public class GcpConfig
{
    public string FirebaseProjectUrl { get; set; }
    public string FirebaseServiceAccountJson { get; set; }
}