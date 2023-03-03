using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Mirror;

public class VerifyPaymentRequestModel : RequestBase
{
    public string Payload { get; set; }
}

public class SubmitPaymentRequestHandler : IRequestHandler<VerifyPaymentRequestModel, ResponseBase>
{
    private readonly TelegramService _telegramService;
    private readonly MirrorDbContext _mirrorDbContext;

    public SubmitPaymentRequestHandler(TelegramService telegramService, MirrorDbContext mirrorDbContext)
    {
        _telegramService = telegramService;
        _mirrorDbContext = mirrorDbContext;
    }

    public async Task<ResponseBase> Handle(VerifyPaymentRequestModel request, CancellationToken cancellationToken)
    {
        var htmlMessage = HtmlMessage.Empty;
        _telegramService.SetupResponse(request);

        var cendolCount = 0;
        var transactionId = string.Empty;
        var userId = request.UserId;
        var expireDate = DateTime.UtcNow;

        if (request.ReplyToMessage != null)
        {
            var messageId = request.ReplyToMessage.MessageId.ToString();
            var fileId = request.ReplyToMessage.GetFileId();
            transactionId = $"{messageId}-{fileId}";

            if (request.Payload.TryConvert(1, out cendolCount))
            {
                var mirrorApproval = await _mirrorDbContext.MirrorApproval
                    .FirstOrDefaultAsync(x =>
                            x.TransactionId == transactionId &&
                            x.Status == (int) EventStatus.Complete,
                        cancellationToken: cancellationToken);

                if (mirrorApproval != null)
                {
                    return await _telegramService.SendMessageText("Sepertinya tiket ini sudah diklaim");
                }

                await _telegramService.SendMessageText("Sedang menambahkan pengguna...");
                _mirrorDbContext.MirrorApproval.Add(new MirrorApprovalEntity()
                {
                    UserId = request.UserId,
                    RawText = request.ReplyToMessage.Text,
                    FileId = request.ReplyToMessage.GetFileId(),
                    Status = (int) EventStatus.Complete,
                    TransactionId = transactionId
                });
            }
            else
            {
                return await _telegramService.SendMessageText("Sertakan durasi berlangganan dengan benar.\nContoh: 1, 2, 3, dst");
            }

            var forwardMessage = request.ReplyToMessage.ForwardFrom;
            if (forwardMessage != null)
            {
                userId = forwardMessage.Id;
            }
        }
        else
        {
            if (string.IsNullOrEmpty(request.Payload))
            {
                return await _telegramService.SendMessageText("Sertakan tautan dari Trakteer.id untuk diverifikasi. Atau balas sebuah pesan untuk menambahkan pengguna.");
            }

            var mirrorApproval = await _mirrorDbContext.MirrorApproval
                .FirstOrDefaultAsync(x =>
                        x.PaymentUrl == request.Payload &&
                        x.Status == (int) EventStatus.Complete,
                    cancellationToken);

            if (mirrorApproval != null)
            {
                return await _telegramService.SendMessageText("Pembayaran sudah terverifikasi.");
            }

            await _telegramService.SendMessageText("Sedang memverifikasi pembayaran. Silakan tunggu...");
            var trakteerParsedDto = await request.Payload.ParseTrakteerWeb();

            if (!trakteerParsedDto.IsValid)
            {
                htmlMessage.BoldBr("Pembayaran gagal diverifikasi.")
                    .Text("Pastikan link yang kamu kirim benar dan bukti pembayaran sudah terverifikasi oleh Trakteer.");
                return await _telegramService.EditMessageText(htmlMessage.ToString());
            }

            _mirrorDbContext.MirrorApproval.Add(new MirrorApprovalEntity()
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
                Status = (int) EventStatus.Complete,
                TransactionId = transactionId
            });

            cendolCount = trakteerParsedDto.CendolCount;
        }

        var mirrorUser = await _mirrorDbContext.MirrorUsers
            .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.Status == (int) EventStatus.Complete,
                cancellationToken: cancellationToken
            );

        expireDate = DateTime.Now.AddMonths(cendolCount);

        if (mirrorUser == null)
        {
            _mirrorDbContext.MirrorUsers.Add(new MirrorUserEntity()
            {
                UserId = userId,
                ExpireDate = expireDate,
                Status = (int) EventStatus.Complete,
                TransactionId = transactionId
            });
        }
        else
        {
            expireDate = mirrorUser.ExpireDate < DateTime.Now
                ? expireDate// If expired, will be started from now
                : mirrorUser.ExpireDate.AddMonths(cendolCount);// If not expired, will be extended from expire date

            mirrorUser.ExpireDate = expireDate;
            mirrorUser.Status = (int) EventStatus.Complete;
            mirrorUser.TransactionId = transactionId;
        }

        await _mirrorDbContext.SaveChangesAsync(cancellationToken);

        htmlMessage.Bold("Pengguna berhasil disimpan").Br()
            .Bold("Langganan sampai: ").Text(expireDate.ToString("yyyy-MM-dd HH:mm:ss")).Br();

        return await _telegramService.EditMessageText(htmlMessage.ToString());
    }
}