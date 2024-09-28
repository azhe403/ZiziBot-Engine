namespace ZiziBot.Application.Handlers.Telegram.Additional;

public class CheckAwbRequest : BotRequestBase
{
}

public class CheckAwbHandler(TelegramService telegramService, BinderByteService binderByteService, TonjooService tonjooService)
    : IRequestHandler<CheckAwbRequest, BotResponseBase>
{
    public async Task<BotResponseBase> Handle(CheckAwbRequest request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);
        var courier = request.MessageText.GetCommandParamAt<string>(0);
        var awb = request.MessageText.GetCommandParamAt<string>(1);

        if (request.MessageTexts?.Length > 2)
        {
            return telegramService.Complete();
        }

        if (courier == null || awb == null)
        {
            return await telegramService.SendMessageText("Masukkan no resi");
        }

        var check = await tonjooService.GetAwbInfoMerged(courier, awb);
        if (check.Contains("tidak ada"))
        {
            check = await binderByteService.CekResiMergedAsync(courier, awb);
        }

        return await telegramService.SendMessageText(check);
    }
}