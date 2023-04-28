using System.ComponentModel;

namespace ZiziBot.Contracts.Configs;

[DisplayName("Mirror")]
public class MirrorConfig
{
    public long ApprovalChannelId { get; set; }
    public string TrakteerVerificationApi { get; set; }
    public int PaymentExpirationDays { get; set; }
}