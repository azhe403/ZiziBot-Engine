using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Mirror;

public class SavePaymentBotRequestModel : BotRequestBase
{
    public string? Payload { get; set; }
    public long ForUserId { get; set; }
}

public class SavePaymentRequestHandler : IRequestHandler<SavePaymentBotRequestModel, BotResponseBase>
{
    private readonly ILogger<SavePaymentRequestHandler> _logger;
    private readonly TelegramService _telegramService;
    private readonly AppSettingRepository _appSettingRepository;
    private readonly MirrorDbContext _mirrorDbContext;

    public SavePaymentRequestHandler(
        ILogger<SavePaymentRequestHandler> logger,
        TelegramService telegramService,
        AppSettingRepository appSettingRepository,
        MirrorDbContext mirrorDbContext
    )
    {
        _logger = logger;
        _telegramService = telegramService;
        _appSettingRepository = appSettingRepository;
        _mirrorDbContext = mirrorDbContext;
    }

    public async Task<BotResponseBase> Handle(SavePaymentBotRequestModel request, CancellationToken cancellationToken)
    {
        var htmlMessage = HtmlMessage.Empty;
        _telegramService.SetupResponse(request);

        var userId = request.UserId;

        if (request.ReplyToMessage == null)
        {
            return await _telegramService.SendMessageText("Balas sebuah pesan untuk menambahkan pengguna.");
        }

        var replyToMessage = request.ReplyToMessage;
        var messageId = replyToMessage.MessageId.ToString();
        var fileId = replyToMessage.GetFileId();
        var transactionId = $"{messageId}-{fileId}";

        if (request.Payload.TryConvert(1, out var cendolCount))
        {
            if (replyToMessage.ForwardSenderName != null &&
                request.ForUserId == 0)
            {
                htmlMessage.Text("Sepertinya Pengguna disembunyikan, spesifikan ID pengguna.").Br()
                    .Bold("Contoh: ").Code($"/mp {cendolCount} (userId)");

                return await _telegramService.SendMessageText(htmlMessage.ToString());
            }

            var mirrorApproval = await _mirrorDbContext.MirrorApproval
                .Where(entity => entity.TransactionId == transactionId)
                .Where(entity => entity.Status == (int)EventStatus.Complete)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (mirrorApproval != null)
            {
                return await _telegramService.SendMessageText("Sepertinya tiket ini sudah diklaim");
            }

            await _telegramService.SendMessageText("Sedang menambahkan pengguna...");
            _mirrorDbContext.MirrorApproval.Add(new MirrorApprovalEntity()
            {
                UserId = request.UserId,
                RawText = request.ReplyToMessage.Text,
                OrderId = messageId,
                FileId = request.ReplyToMessage.GetFileId(),
                Status = (int)EventStatus.Complete,
                TransactionId = transactionId
            });
        }
        else
        {
            htmlMessage.Text("Sertakan durasi berlangganan dengan benar.").Br()
                .Bold("Contoh: ").Code("/mp (duration) [userId]");

            return await _telegramService.SendMessageText(htmlMessage.ToString());
        }

        if (request.ForUserId != 0)
        {
            userId = request.ForUserId;
        }

        var forwardMessage = replyToMessage.ForwardFrom;
        if (forwardMessage != null)
        {
            userId = forwardMessage.Id;
        }

        var mirrorUser = await _mirrorDbContext.MirrorUsers
            .Where(entity => entity.UserId == userId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        var expireDate = DateTime.Now.AddMonths(cendolCount);

        if (mirrorUser == null)
        {
            _logger.LogInformation("Creating Mirror subscription for user {UserId} with Expire date: {Date}", userId, expireDate);

            _mirrorDbContext.MirrorUsers.Add(new MirrorUserEntity()
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
                : mirrorUser.ExpireDate.AddMonths(cendolCount); // If not expired, it will be extended from current expire date

            mirrorUser.ExpireDate = expireDate;
            mirrorUser.Status = (int)EventStatus.Complete;
            mirrorUser.TransactionId = transactionId;
        }

        await _mirrorDbContext.SaveChangesAsync(cancellationToken);

        htmlMessage.Bold("Pengguna berhasil disimpan").Br()
            .Bold("ID Pengguna: ").Code(userId.ToString()).Br()
            .Bold("Jumlah Cendol: ").Code(cendolCount.ToString()).Br()
            .Bold("Langganan sampai: ").Code(expireDate.AddHours(Env.DEFAULT_TIMEZONE).ToString("yyyy-MM-dd HH:mm:ss")).Br();

        await _telegramService.EditMessageText(htmlMessage.ToString());

        var mirrorConfig = await _appSettingRepository.GetConfigSectionAsync<MirrorConfig>();

        return await _telegramService.SendMessageText(htmlMessage.ToString(), chatId: mirrorConfig.ApprovalChannelId);
    }
}