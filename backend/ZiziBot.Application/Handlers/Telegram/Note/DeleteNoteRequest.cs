namespace ZiziBot.Application.Handlers.Telegram.Note;

public class DeleteNoteRequest : BotRequestBase
{
    public string Note { get; set; }
}

public class DeleteNoteRequestHandler : IRequestHandler<DeleteNoteRequest, BotResponseBase>
{
    private readonly TelegramService _telegramService;
    private readonly NoteService _noteService;

    public DeleteNoteRequestHandler(TelegramService telegramService, NoteService noteService)
    {
        _telegramService = telegramService;
        _noteService = noteService;
    }

    public async Task<BotResponseBase> Handle(DeleteNoteRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        return await _noteService.Delete(request.ChatIdentifier, request.Note, async message => await _telegramService.SendMessageText(message));
    }
}