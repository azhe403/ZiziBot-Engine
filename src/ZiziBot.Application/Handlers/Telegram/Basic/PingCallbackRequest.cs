using Telegram.Bot;

namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class PingCallbackRequestModel : RequestBase
{
}

public class PingCallbackRequestHandler : IRequestHandler<PingCallbackRequestModel, ResponseBase>
{
    private readonly SudoService _sudoService;

    public PingCallbackRequestHandler(SudoService sudoService)
    {
        _sudoService = sudoService;
    }

    public async Task<ResponseBase> Handle(PingCallbackRequestModel request, CancellationToken cancellationToken)
    {
        ResponseBase responseBase = new(request);

        if (!await _sudoService.IsSudoAsync(request.UserId))
        {
            return await responseBase.AnswerCallbackAsync("Kamu tidak memiliki akses");
        }

        var webhookInfo = await responseBase.Bot.GetWebhookInfoAsync(cancellationToken: cancellationToken);

        var htmlMessage = HtmlMessage.Empty;

        if (webhookInfo.Url != null)
        {
            htmlMessage.Text("URL: ").TextBr(webhookInfo.Url);
        }
        else
        {
            htmlMessage.Text("Engine mode bukan Webhook");
        }

        return await responseBase.AnswerCallbackAsync(webhookInfo.Url, showAlert: true);
    }
}