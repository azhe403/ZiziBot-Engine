using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Mirror;

public class VerifyPaymentRequestModel : RequestBase
{
    public string PaymentUrl { get; set; }
}

public class SubmitPaymentRequestHandler : IRequestHandler<VerifyPaymentRequestModel, ResponseBase>
{
    private readonly MirrorDbContext _mirrorDbContext;

    public SubmitPaymentRequestHandler(MirrorDbContext mirrorDbContext)
    {
        _mirrorDbContext = mirrorDbContext;
    }

    public async Task<ResponseBase> Handle(VerifyPaymentRequestModel request, CancellationToken cancellationToken)
    {
        ResponseBase responseBase = new(request);

        if (string.IsNullOrEmpty(request.PaymentUrl))
        {
            return await responseBase.SendMessageText("Silakan balas pesan yang berisi Screenshot bukti pembayaran.");
        }

        var mirrorApproval = await _mirrorDbContext.MirrorApproval
            .FirstOrDefaultAsync(
                x =>
                    x.PaymentUrl == request.PaymentUrl &&
                    x.Status == (int) EventStatus.Complete,
                cancellationToken
            );

        if (mirrorApproval != null)
        {
            return await responseBase.SendMessageText("Pembayaran sudah terverifikasi.");
        }

        await responseBase.SendMessageText("Sedang memverifikasi pembayaran. Silakan tunggu...");
        var payment = await request.PaymentUrl.ParseTrakteerWeb();

        _mirrorDbContext.MirrorApproval.Add(
            new MirrorApprovalEntity()
            {
                UserId = request.UserId,
                PaymentUrl = payment.PaymentUrl,
                RawText = payment.RawText,
                CendolsCount = payment.CendolsCount,
                Cendols = payment.Cendols,
                AdminFees = payment.AdminFees,
                Subtotal = payment.Subtotal,
                OrderDate = payment.OrderDate,
                PaymentMethod = payment.PaymentMethod,
                OrderId = payment.OrderId,
                Status = (int) EventStatus.Complete
            }
        );

        await _mirrorDbContext.SaveChangesAsync(cancellationToken);

        var htmlMessage = HtmlMessage.Empty
            .Bold("Pembayaran Diterima").Br()
            .Text("Silakan tekan /ms untuk memeriksa.").Br();

        return await responseBase.EditMessageText(htmlMessage.ToString());
    }
}