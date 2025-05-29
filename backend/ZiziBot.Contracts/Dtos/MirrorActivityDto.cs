using ZiziBot.Contracts.Enums;

namespace ZiziBot.Contracts.Dtos;

public class MirrorActivityDto
{
    public long UserId { get; set; }
    public MirrorActivityType ActivityTypeId { get; set; }
    public string ActivityName { get; set; }
    public string TransactionId { get; set; }
    public string Url { get; set; }
}