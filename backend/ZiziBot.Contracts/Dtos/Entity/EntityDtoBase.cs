namespace ZiziBot.Contracts.Dtos.Entity;

public class EntityDtoBase
{
    public int Status { get; set; }
    public string? TransactionId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}