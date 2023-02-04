namespace ZiziBot.Application.Handlers.Telegram.Note;

public class GetNoteRequestModel : RequestBase
{
}

public class GetNoteRequestHandler : IRequestHandler<GetNoteRequestModel, ResponseBase>
{
    private readonly NoteService _noteService;

    public GetNoteRequestHandler(NoteService noteService)
    {
        _noteService = noteService;
    }

    public async Task<ResponseBase> Handle(GetNoteRequestModel request, CancellationToken cancellationToken)
    {
        ResponseBase responseBase = new(request);

        var htmlMessage = HtmlMessage.Empty;

        var tags = await _noteService.GetAllByChat(request.ChatIdentifier);

        if (tags.Count == 0)
        {
            htmlMessage.Text("Tidak ada catatan di Obrolan ini.");
        }
        else
        {
            htmlMessage.Bold("Daftar Catatan:").Br();
            tags.ForEach(entity => htmlMessage.Text(entity.Query).Br());
        }

        return await responseBase.SendMessageText(htmlMessage.ToString());
    }
}