namespace ZiziBot.Contracts.Dtos;

public class TrakteerParsedDto
{
    public bool IsValid { get; set; }
    public string? OrderId { get; set; }
    public string PaymentUrl { get; set; }
    public DateTime OrderDate { get; set; }
    public string? PaymentMethod { get; set; }
    public int CendolCount { get; set; }
    public string? Cendols { get; set; }
    public int AdminFees { get; set; }
    public int Subtotal { get; set; }
    public string? RawText { get; set; }
}

public class TrakteerApiDto
{
    public bool IsValid { get; set; }
    public string? OrderId { get; set; }
    public string PaymentUrl { get; set; }
    public int CendolCount { get; set; }
    public int AdminFees { get; set; }
    public int Total { get; set; }
    public DateTime OrderDate { get; set; }
    public string? RawText { get; set; }
    public string? PaymentMethod { get; set; }
}