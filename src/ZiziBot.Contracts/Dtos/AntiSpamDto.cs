namespace ZiziBot.Contracts.Dtos;

public class AntiSpamDto
{
    public bool IsBanAny { get; set; }
    public bool IsBanEss { get; set; }
    public bool IsBanCasFed { get; set; }
    public bool IsBanSwFed { get; set; }
    public CombotAntispamResult? CasRecord { get; set; }
}