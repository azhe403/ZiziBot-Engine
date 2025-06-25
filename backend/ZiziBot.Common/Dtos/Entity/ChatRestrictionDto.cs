namespace ZiziBot.Common.Dtos.Entity;

public class ChatRestrictionDto : EntityDtoBase
{
    public long UserId { get; set; }
    public long ChatId { get; set; }
}