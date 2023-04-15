namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("MirrorApproval")]
public class MirrorApprovalEntity : EntityBase
{
    public long UserId { get; set; }
    public string PaymentUrl { get; set; }
    public string? RawText { get; set; }
    public int CendolCount { get; set; }
    public string? Cendols { get; set; }
    public string? AdminFees { get; set; }
    public string? Subtotal { get; set; }
    public DateTime OrderDate { get; set; }
    public string? PaymentMethod { get; set; }
    public string? OrderId { get; set; }
    public string? FileId { get; set; }
}