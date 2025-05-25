using System.Security.Cryptography;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class PrepareConsoleBotRequest : BotRequestBase
{ }

public class PrepareConsoleHandler(
    ServiceFacade serviceFacade,
    DataFacade dataFacade
) : IBotRequestHandler<PrepareConsoleBotRequest>
{
    public async Task<BotResponseBase> Handle(PrepareConsoleBotRequest request, CancellationToken cancellationToken)
    {
        serviceFacade.TelegramService.SetupResponse(request);

        var sessionId = Guid.NewGuid().ToString();
        var consoleUrl = EnvUtil.GetEnv(Env.WEB_CONSOLE_URL);
        var webUrlBase = consoleUrl + "?session_id=";
        var webUrl = webUrlBase + sessionId;

        if (!EnvUtil.IsEnvExist(Env.WEB_CONSOLE_URL))
        {
            await serviceFacade.TelegramService.SendMessageText("Maaf fitur ini belum dipersiapkan");
        }

        var replyMarkup = InlineKeyboardMarkup.Empty();
        var htmlMessage = HtmlMessage.Empty
            .BoldBr("🎛 ZiziBot Console")
            .TextBr("Buka Console untuk mengelola pengaturan, catatan dan lain-lain.")
            .Br();

        if (request.IsPrivateChat)
        {
            var otp = RandomNumberGenerator.GetInt32(100000, 999999);
            dataFacade.MongoEf.UserOtp.Add(new UserOtpEntity() {
                UserId = request.UserId,
                Otp = otp,
                Status = EventStatus.InProgress,
                TransactionId = request.TransactionId,
                CreatedBy = request.UserId,
                UpdatedBy = request.UserId
            });

            await dataFacade.MongoEf.SaveChangesAsync();
        }

        if (webUrl.Contains("localhost"))
        {
            htmlMessage.Code(webUrl).Br();
        }
        else
        {
            replyMarkup = new[] {
                new[] {
                    InlineKeyboardButton.WithLoginUrl("Buka Console", new LoginUrl() {
                        Url = webUrl
                    })
                }
            }.ToButtonMarkup();
        }

        return await serviceFacade.TelegramService.SendMessageText(htmlMessage.ToString(), replyMarkup);
    }
}