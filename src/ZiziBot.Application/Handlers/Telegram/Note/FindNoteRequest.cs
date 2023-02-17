using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Telegram.Note;

public class FindNoteRequest : RequestBase
{

}

public class FindNoteRequestHandler : IRequestHandler<FindNoteRequest, ResponseBase>
{
    private readonly ILogger<FindNoteRequestHandler> _logger;
    private readonly NoteService _noteService;

    public FindNoteRequestHandler(ILogger<FindNoteRequestHandler> logger, NoteService noteService)
    {
        _logger = logger;
        _noteService = noteService;
    }

    public async Task<ResponseBase> Handle(FindNoteRequest request, CancellationToken cancellationToken)
    {
        var response = new ResponseBase(request);
        var listNote = await _noteService.GetAllByChat(request.ChatIdentifier);
        if (listNote.Count == 0)
        {
            _logger.LogInformation("No note found in chat {chatId}", request.ChatIdentifier);
            return response.Complete();
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
                await response.SendMessageText(notes.Content);
            }
        }
        else
        {
            //find note by text (invoked by entire message text)
            var note = listNote.FirstOrDefault(x => x.Query == request.MessageText);
            if (note != null)
            {
                await response.SendMessageText(note.Content);
            }
        }

        return response.Complete();
    }
}