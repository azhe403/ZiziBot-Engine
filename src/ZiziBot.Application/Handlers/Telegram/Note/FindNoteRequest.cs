using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Telegram.Note;

public class FindNoteRequest : RequestBase
{

}

public class FindNoteRequestHandler : IRequestHandler<FindNoteRequest, ResponseBase>
{
    private readonly ILogger<FindNoteRequestHandler> _logger;
    private readonly TelegramService _telegramService;
    private readonly NoteService _noteService;

    public FindNoteRequestHandler(ILogger<FindNoteRequestHandler> logger, TelegramService telegramService, NoteService noteService)
    {
        _logger = logger;
        _telegramService = telegramService;
        _noteService = noteService;
    }

    public async Task<ResponseBase> Handle(FindNoteRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        var listNote = await _noteService.GetAllByChat(request.ChatIdentifier);
        if (listNote.Count == 0)
        {
            _logger.LogInformation("No note found in chat {chatId}", request.ChatIdentifier);
            return _telegramService.Complete();
        }

        var words = request.MessageTexts?.Where(x => x.StartsWith('#')).ToList();
        if (words?.Any() ?? false)
        {
            //find note single word (invoked by #note)
            foreach (var notes in words
                         .Select(word => word.Replace("#", ""))
                         .Select(tag => listNote.FirstOrDefault(x => x.Query == tag))
                         .Where(notes => notes != null)
                    )
            {
                await SendNoteAsync(notes);
            }
        }
        else
        {
            //find note by text (invoked by entire message text)
            var note = listNote.FirstOrDefault(x => x.Query == request.MessageText);
            await SendNoteAsync(note);

        }

        return _telegramService.Complete();
    }

    private async Task SendNoteAsync(NoteEntity? notes)
    {
        if (notes == null) return;

        var dataType = (CommonMediaType) notes.DataType;

        _logger.LogInformation("Sending note {noteId} with data type {dataType}", notes.Id, dataType);

        var replyMarkup = notes.RawButton.ToButtonMarkup();

        if (dataType <= CommonMediaType.Text)
            await _telegramService.SendMessageText(notes.Content, replyMarkup: replyMarkup);
        else
            await _telegramService.SendMediaAsync(
                fileId: notes.FileId,
                caption: notes.Content,
                mediaType: (CommonMediaType) notes.DataType,
                replyMarkup: replyMarkup
            );
    }
}