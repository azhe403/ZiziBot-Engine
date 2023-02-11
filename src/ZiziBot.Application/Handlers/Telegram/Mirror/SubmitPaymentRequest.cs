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
        var htmlMessage = HtmlMessage.Empty;
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
        var trakteerParsedDto = await request.PaymentUrl.ParseTrakteerWeb();

        if (!trakteerParsedDto.IsValid)
        {
            htmlMessage.BoldBr("Pembayaran gagal diverifikasi.")
                .Text("Pastikan link yang kamu kirim benar dan bukti pembayaran sudah terverifikasi oleh Trakteer.");
            return await responseBase.EditMessageText(htmlMessage.ToString());
        }

        _mirrorDbContext.MirrorApproval.Add(
            new MirrorApprovalEntity()
            {
                UserId = request.UserId,
                PaymentUrl = trakteerParsedDto.PaymentUrl,
                RawText = trakteerParsedDto.RawText,
                CendolCount = trakteerParsedDto.CendolCount,
                Cendols = trakteerParsedDto.Cendols,
                AdminFees = trakteerParsedDto.AdminFees,
                Subtotal = trakteerParsedDto.Subtotal,
                OrderDate = trakteerParsedDto.OrderDate,
                PaymentMethod = trakteerParsedDto.PaymentMethod,
                OrderId = trakteerParsedDto.OrderId,
                Status = (int) EventStatus.Complete
            }
        );

        var mirrorUser = await _mirrorDbContext.MirrorUsers
            .FirstOrDefaultAsync(
                x =>
                    x.UserId == request.UserId &&
                    x.Status == (int) EventStatus.Complete,
                cancellationToken: cancellationToken
            );

        if (mirrorUser == null)
        {
            _mirrorDbContext.MirrorUsers.Add(
                new MirrorUserEntity()
                {
                    UserId = request.UserId,
                    ExpireDate = DateTime.Now.AddMonths(trakteerParsedDto.CendolCount),
                    Status = (int) EventStatus.Complete
                }
            );
        }
        else
        {
            mirrorUser.ExpireDate = mirrorUser.ExpireDate < DateTime.Now
                ? DateTime.Now.AddMonths(trakteerParsedDto.CendolCount)// If expired, will be started from now
                : mirrorUser.ExpireDate.AddMonths(trakteerParsedDto.CendolCount);// If not expired, will be extended from expire date
        }

        await _mirrorDbContext.SaveChangesAsync(cancellationToken);

        htmlMessage.Bold("Pembayaran Diterima").Br()
            .Text("Silakan tekan /ms untuk memeriksa.").Br();

        return await responseBase.EditMessageText(htmlMessage.ToString());
    }
}