namespace ZiziBot.Application.Handlers.Telegram.Text;

public class OcrBotRequest : BotRequestBase
{
}

public class OcrRequestHandler : IBotRequestHandler<OcrBotRequest>
{
    private readonly TelegramService _telegramService;
    private readonly OptiicDevService _optiicDevService;

    public OcrRequestHandler(TelegramService telegramService, OptiicDevService optiicDevService)
    {
        _telegramService = telegramService;
        _optiicDevService = optiicDevService;
    }

    public async Task<BotResponseBase> Handle(OcrBotRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        await _telegramService.SendMessageText("🔎 Sedang melakukan OCR");

        try
        {
            var localFile = await _telegramService.DownloadFileAsync("ocr/");
            var ocrResult = await _optiicDevService.ScanImageAsync(localFile);

            return await _telegramService.EditMessageText(ocrResult.Text);
        }
        catch (Exception exception)
        {
            var htmlMessage = HtmlMessage.Empty
                .Bold("Terjadi kesalahan ketika menjalankan OCR").Br()
                .Bold("Error: ").Text(exception.Message.Split(":").FirstOrDefault());

            return await _telegramService.EditMessageText(htmlMessage.ToString());
        }
    }
}