using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.UseCases.Mirror;

public struct DonationSettlementRequest
{
    public string OrderId { get; set; }
}

public class DonationSettlementValidator : AbstractValidator<DonationSettlementRequest>
{ }

public class DonationSettlementResponse
{
    public ParseMethod Method { get; set; }
    public bool IsValid { get; set; }
    public DonationSource Source { get; set; }
    public string? OrderId { get; set; }
    public string PaymentUrl { get; set; }
    public DateTime OrderDate { get; set; }
    public string? PaymentMethod { get; set; }
    public int CendolCount => Subtotal / ValueConst.CENDOL_PRICE;
    public string? Cendols { get; set; }
    public int AdminFees { get; set; }
    public int Subtotal { get; set; }
    public string? RawText { get; set; }
}

public class DonationSettlementUseCase(ILogger<DonationSettlementUseCase> logger, DonationSettlementValidator validator, MirrorPaymentService mirrorPaymentService)
{
    public async Task<DonationSettlementResponse> Handle(DonationSettlementRequest request)
    {
        logger.LogInformation("Reading information for OrderId: {OrderId}", request.OrderId);

        using var cts = new CancellationTokenSource();
        var trakteerApiTask = mirrorPaymentService.GetTrakteerApi(request.OrderId, cts.Token);
        var trakteerWebTask = mirrorPaymentService.ParseTrakteerWeb(request.OrderId, cts.Token);

        var saweriaApiTask = mirrorPaymentService.GetSaweriaApi(request.OrderId, cts.Token);
        var saweriaWebTask = mirrorPaymentService.ParseSaweriaWeb(request.OrderId, cts.Token);

        var parsedDonationTask = await Task.WhenAny(trakteerApiTask, trakteerWebTask, saweriaApiTask, saweriaWebTask);
        var parsedDonationDto = await parsedDonationTask;

        logger.LogDebug("OrderId: {OrderId} from Source: {Source} is Valid: {IsValid}", request.OrderId, parsedDonationDto.Source, parsedDonationDto.IsValid);
        await cts.CancelAsync();

        var donationSettlement = new DonationSettlementResponse() {
            Method = parsedDonationDto.Method,
            IsValid = parsedDonationDto.IsValid,
            Source = parsedDonationDto.Source,
            OrderId = parsedDonationDto.OrderId,
            PaymentUrl = parsedDonationDto.PaymentUrl,
            OrderDate = parsedDonationDto.OrderDate,
            PaymentMethod = parsedDonationDto.PaymentMethod,
            Cendols = parsedDonationDto.Cendols,
            Subtotal = parsedDonationDto.Subtotal,
            AdminFees = parsedDonationDto.AdminFees,
            RawText = parsedDonationDto.RawText
        };

        return donationSettlement;
    }
}