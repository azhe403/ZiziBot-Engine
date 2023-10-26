namespace ZiziBot.Contracts.Dtos;

public class WordFilterDto
{
    public required string Id { get; set; }
    public long ChatId { get; set; }
    public long UserId { get; set; }
    public required string Word { get; set; }
    public bool IsGlobal { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}