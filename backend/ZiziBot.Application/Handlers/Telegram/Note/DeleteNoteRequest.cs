namespace ZiziBot.Application.Handlers.Telegram.Note;

public class DeleteNoteRequest : BotRequestBase
{
    public string Note { get; set; }
}

public class DeleteNoteRequestHandler(TelegramService telegramService, NoteService noteService) : IRequestHandler<DeleteNoteRequest, BotResponseBase>
{
    public async Task<BotResponseBase> Handle(DeleteNoteRequest request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        return await noteService.Delete(request.ChatIdentifier, request.Note, async message => await telegramService.SendMessageText(message));
    }
}