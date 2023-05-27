namespace ZiziBot.Application.Handlers.Telegram.Additional;

public class CheckAwbRequest : BotRequestBase
{
}

public class CheckAwbHandler : IRequestHandler<CheckAwbRequest, BotResponseBase>
{
    private readonly TelegramService _telegramService;
    private readonly BinderByteService _binderByteService;
    private readonly TonjooService _tonjooService;

    public CheckAwbHandler(TelegramService telegramService, BinderByteService binderByteService, TonjooService tonjooService)
    {
        _telegramService = telegramService;
        _binderByteService = binderByteService;
        _tonjooService = tonjooService;
    }

    public async Task<BotResponseBase> Handle(CheckAwbRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);
        var courier = request.MessageText.GetCommandParamAt<string>(0);
        var awb = request.MessageText.GetCommandParamAt<string>(1);

        if (courier == null || awb == null)
        {
            return await _telegramService.SendMessageText("Masukkan no resi");
        }

        // var check = await _binderByteService.CekResiMergedAsync(courier, awb);
        var check = await _tonjooService.GetAwbInfoMerged(courier, awb);

        return await _telegramService.SendMessageText(check);
    }
}