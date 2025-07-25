using ZiziBot.Common.Enums;

namespace ZiziBot.Common.Dtos;

public class WordFilterDto
{
    public string Id { get; set; }
    public long ChatId { get; set; }
    public long UserId { get; set; }
    public required string Word { get; set; }
    public bool IsGlobal { get; set; }
    public bool IsRegex { get; set; }
    public PipelineResultAction[]? Action { get; set; }
    public string TransactionId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}