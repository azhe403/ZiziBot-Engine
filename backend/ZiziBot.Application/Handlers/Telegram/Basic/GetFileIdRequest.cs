using Telegram.Bot.Types.Enums;

namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class GetFileIdRequest : RequestBase
{
}

public class GetFileIdHandler : IRequestHandler<GetFileIdRequest, ResponseBase>
{
    private readonly TelegramService _telegramService;

    public GetFileIdHandler(TelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public async Task<ResponseBase> Handle(GetFileIdRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        var htmlMessage = HtmlMessage.Empty;

        if (request.ReplyToMessage == null)
        {
            return await _telegramService.SendMessageText("Balas sebuah pesan untuk mendapatkan File ID-nya");
        }

        if (request.ChatType != ChatType.Private)
        {
            htmlMessage.BoldBr($"👥 {request.ChatTitle}")
                .Bold("Chat ID: ").CodeBr(request.ChatId.ToString())
                .Br();
        }
        var replyToMessage = request.ReplyToMessage;
        var fileId = replyToMessage.GetFileId();

        if (fileId.IsNullOrEmpty())
        {
            return await _telegramService.SendMessageText("Tidak dapat menemukan File ID");
        }

        htmlMessage.Bold("FileId: ").CodeBr(fileId)
            .Bold("Type: ").CodeBr(replyToMessage.Type.ToString());


        return await _telegramService.SendMessageText(htmlMessage.ToString());
    }
}