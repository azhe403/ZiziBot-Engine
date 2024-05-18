namespace ZiziBot.Application.Handlers.Telegram.Text;

public class OcrBotRequest : BotRequestBase
{
}

public class OcrRequestHandler(
    TelegramService telegramService,
    OptiicDevService optiicDevService,
    OcrSpaceService ocrSpaceService
) : IBotRequestHandler<OcrBotRequest>
{
    public async Task<BotResponseBase> Handle(OcrBotRequest request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        await telegramService.SendMessageText("🔎 Sedang melakukan OCR");

        try
        {
            var localFile = await telegramService.DownloadFileAsync("ocr/");
            // var ocrResult = await _optiicDevService.ScanImageAsync(localFile);
            var result = await ocrSpaceService.ParseImage(localFile);

            return await telegramService.EditMessageText(result);
        }
        catch (Exception exception)
        {
            var htmlMessage = HtmlMessage.Empty
                .Bold("Terjadi kesalahan ketika menjalankan OCR").Br()
                .Bold("Error: ").Text(exception.Message.Split(":").FirstOrDefault());

            return await telegramService.EditMessageText(htmlMessage.ToString());
        }
    }
}