namespace ZiziBot.Application.Handlers.Telegram.Additional;

public class CheckResiRequest : BotRequestBase
{
}

public class CheckResiHandler : IRequestHandler<CheckResiRequest, BotResponseBase>
{
    private readonly TelegramService _telegramService;
    private readonly BinderByteService _binderByteService;

    public CheckResiHandler(TelegramService telegramService, BinderByteService binderByteService)
    {
        _telegramService = telegramService;
        _binderByteService = binderByteService;
    }

    public async Task<BotResponseBase> Handle(CheckResiRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);
        var courier = request.MessageText.GetCommandParamAt<string>(0);
        var awb = request.MessageText.GetCommandParamAt<string>(1);

        if (courier == null || awb == null)
        {
            return await _telegramService.SendMessageText("Masukkan no resi");
        }

        var check = await _binderByteService.CekResiMergedAsync(courier, awb);

        return await _telegramService.SendMessageText(check);
    }
}