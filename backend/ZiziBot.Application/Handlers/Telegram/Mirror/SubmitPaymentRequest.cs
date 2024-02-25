using Microsoft.Extensions.Logging;
using MongoFramework.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace ZiziBot.Application.Handlers.Telegram.Mirror;

public class SubmitPaymentBotRequest : BotRequestBase
{
    public string? Payload { get; set; }
    public long ForUserId { get; set; }
}

public class SubmitPaymentBotRequestHandler : IBotRequestHandler<SubmitPaymentBotRequest>
{
    private readonly ILogger<SubmitPaymentBotRequestHandler> _logger;
    private readonly TelegramService _telegramService;
    private readonly MongoDbContextBase _mongoDbContext;
    private readonly MirrorPaymentService _mirrorPaymentService;
    private readonly AppSettingRepository _appSettingRepository;

    public SubmitPaymentBotRequestHandler(
        ILogger<SubmitPaymentBotRequestHandler> logger,
        TelegramService telegramService,
        AppSettingRepository appSettingRepository,
        MongoDbContextBase mongoDbContext,
        MirrorPaymentService mirrorPaymentService
    )
    {
        _logger = logger;
        _telegramService = telegramService;
        _appSettingRepository = appSettingRepository;
        _mongoDbContext = mongoDbContext;
        _mirrorPaymentService = mirrorPaymentService;
    }

    public async Task<BotResponseBase> Handle(SubmitPaymentBotRequest request, CancellationToken cancellationToken)
    {
        var htmlMessage = HtmlMessage.Empty;
        _telegramService.SetupResponse(request);

        var transactionId = string.Empty;
        var userId = request.UserId;

        var replyMarkup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithUrl("Bagaimana cara mendapatkan OrderId?", UrlConst.DOC_MIRROR_VERIFY_DONATION)
            }
        });

        var guidOrderId = request.Payload?.UrlSegment(1, request.Payload);

        if (guidOrderId.IsNullOrEmpty())
        {
            htmlMessage.Text("Sertakan Order Id untuk diverifikasi.").Br()
                .Bold("Contoh: ").CodeBr("/sp 9d16023b-67be-5a0b-bc47-47809f059013");


            return await _telegramService.SendMessageText(htmlMessage.ToString(), replyMarkup);
        }

        if (!guidOrderId.IsValidGuid())
        {
            htmlMessage = HtmlMessage.Empty
                .Bold("OrderId sepertinya tidak valid").Br()
                .Text("Pastikan <b>OrderId</b> Anda dapatkan dari Trakteer/Saweria");

            return await _telegramService.SendMessageText(htmlMessage.ToString(), replyMarkup);
        }

        if (request.ForUserId != 0)
        {
            userId = request.ForUserId;
        }

        await _telegramService.SendMessageText("Sedang memverifikasi pembayaran. Silakan tunggu...");
        var trakteerParsedDto = await _mirrorPaymentService.GetTrakteerApi(guidOrderId);

        var orderId = trakteerParsedDto.OrderId;
        var orderDate = trakteerParsedDto.OrderDate;
        var cendolCount = trakteerParsedDto.CendolCount;
        var total = trakteerParsedDto.Total;
        var paymentUrl = trakteerParsedDto.PaymentUrl;
        var donationSource = "Trakteer";

        if (!trakteerParsedDto.IsValid)
        {
            var saweriaParsedDto = await _mirrorPaymentService.GetSaweriaApi(guidOrderId);

            orderId = saweriaParsedDto.OrderId;
            orderDate = saweriaParsedDto.OrderDate;
            cendolCount = saweriaParsedDto.CendolCount;
            total = saweriaParsedDto.Total;
            paymentUrl = saweriaParsedDto.PaymentUrl;
            donationSource = "Saweria";
        }

        if (orderId == null)
        {
            htmlMessage.BoldBr("Pembayaran gagal diverifikasi.")
                .Text("Pastikan Order Id yang kamu dapatkan dari Trakteer/Saweria.");

            return await _telegramService.EditMessageText(htmlMessage.ToString());
        }

        var mirrorConfig = await _appSettingRepository.GetConfigSectionAsync<MirrorConfig>();

        if (orderDate <= DateTime.UtcNow.AddHours(Env.DEFAULT_TIMEZONE).AddDays(-mirrorConfig!.PaymentExpirationDays))
        {
            return await _telegramService.EditMessageText("Bukti pembayaran sudah kadaluarsa. Silakan lakukan pembayaran ulang.");
        }

        var mirrorApproval = await _mongoDbContext.MirrorApproval
            .Where(entity => entity.OrderId == orderId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken);

        if (mirrorApproval != null)
        {
            htmlMessage = HtmlMessage.Empty
                .Bold("Pembayaran ini sudah diklaim.").Br()
                .Text("Pastikan <b>OrderId</b> Anda dapatkan dari Trakteer/Saweria");

            return await _telegramService.EditMessageText(htmlMessage.ToString(), replyMarkup);
        }

        _mongoDbContext.MirrorApproval.Add(new MirrorApprovalEntity()
        {
            UserId = request.UserId,
            PaymentUrl = paymentUrl,
            RawText = trakteerParsedDto.RawText,
            CendolCount = cendolCount,
            AdminFees = trakteerParsedDto.AdminFees,
            Subtotal = total,
            OrderDate = orderDate,
            PaymentMethod = trakteerParsedDto.PaymentMethod,
            OrderId = orderId,
            Status = (int)EventStatus.Complete,
            TransactionId = transactionId
        });

        var mirrorUser = await _mongoDbContext.MirrorUsers
            .Where(entity => entity.UserId == userId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        var expireDate = DateTime.Now.AddMonths(cendolCount);

        if (mirrorUser == null)
        {
            _logger.LogInformation("Creating Mirror subscription for user {UserId} with Expire date: {Date}", userId, expireDate);

            _mongoDbContext.MirrorUsers.Add(new MirrorUserEntity()
            {
                UserId = userId,
                ExpireDate = expireDate,
                Status = (int)EventStatus.Complete,
                TransactionId = transactionId
            });
        }
        else
        {
            _logger.LogInformation("Extending Mirror subscription for user {UserId} with Expire date: {Date}", userId, expireDate);

            expireDate = mirrorUser.ExpireDate < DateTime.Now
                ? expireDate // If expired, will be started from now
                : mirrorUser.ExpireDate.AddMonths(cendolCount); // If not expired, will be extended from expire date

            mirrorUser.ExpireDate = expireDate;
            mirrorUser.Status = (int)EventStatus.Complete;
            mirrorUser.TransactionId = transactionId;
        }

        await _mongoDbContext.SaveChangesAsync(cancellationToken);

        htmlMessage.Bold("Langganan berhasil disimpan").Br()
            .Bold("ID Pengguna: ").Code(userId.ToString()).Br()
            .Bold("Pengguna: ").UserMention(request.User).Br()
            .Bold("Jumlah Cendol: ").Code(cendolCount.ToString()).Br()
            .Bold("Sumber: ").Code(donationSource).Br()
            .Bold("Langganan sampai: ").Code(expireDate.AddHours(Env.DEFAULT_TIMEZONE).ToString("yyyy-MM-dd HH:mm:ss zzz")).Br();

        await _telegramService.EditMessageText(htmlMessage.ToString());

        htmlMessage.Bold("OrderID: ").CodeBr(orderId)
            .Bold("Url: ").Text(paymentUrl);

        return await _telegramService.SendMessageText(htmlMessage.ToString(), chatId: mirrorConfig.ApprovalChannelId, threadId: 0);
    }
}