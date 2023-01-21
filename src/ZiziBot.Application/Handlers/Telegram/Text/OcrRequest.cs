namespace ZiziBot.Application.Handlers.Telegram.Text;

public class OcrRequestModel : RequestBase
{
}

public class OcrRequestHandler : IRequestHandler<OcrRequestModel, ResponseBase>
{
    private readonly OptiicDevService _optiicDevService;

    public OcrRequestHandler(OptiicDevService optiicDevService)
    {
        _optiicDevService = optiicDevService;
    }

    public async Task<ResponseBase> Handle(OcrRequestModel request, CancellationToken cancellationToken)
    {
        ResponseBase responseBase = new(request);

        await responseBase.SendMessageText("🔎 Sedang melakukan OCR");

        try
        {

            var localFile = await responseBase.DownloadFileAsync("ocr_");
            var ocrResult = await _optiicDevService.ScanImageAsync(localFile);

            return await responseBase.EditMessageText(ocrResult.Text);
        }
        catch (Exception e)
        {
            var htmlMessage = HtmlMessage.Empty
                .Bold("Terjadi kesalahan ketika menjalankan OCR").Br()
                .Bold("Error: ").Text(e.Message.Split(":").FirstOrDefault());

            return await responseBase.EditMessageText(htmlMessage.ToString());
        }
    }
}