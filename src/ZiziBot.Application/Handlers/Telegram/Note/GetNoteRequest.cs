namespace ZiziBot.Application.Handlers.Telegram.Note;

public class GetNoteRequestModel : RequestBase
{
}

public class GetNoteRequestHandler : IRequestHandler<GetNoteRequestModel, ResponseBase>
{
    private readonly TelegramService _telegramService;
    private readonly NoteService _noteService;

    public GetNoteRequestHandler(TelegramService telegramService, NoteService noteService)
    {
        _telegramService = telegramService;
        _noteService = noteService;
    }

    public async Task<ResponseBase> Handle(GetNoteRequestModel request, CancellationToken cancellationToken)
    {
        var htmlMessage = HtmlMessage.Empty;
        _telegramService.SetupResponse(request);

        var tags = await _noteService.GetAllByChat(request.ChatIdentifier);

        if (tags.Count == 0)
        {
            htmlMessage.Text("Tidak ada catatan di Obrolan ini.");
        }
        else
        {
            htmlMessage.Bold("Daftar Tagar:").Br();
            var grouped = tags
                .GroupBy(entity => entity.Query.Split(" ").Length <= 1)
                .ToList();

            grouped.First(x => x.Key == true)
                .ToList()
                .ForEach(entity => htmlMessage.Text("#").Text(entity.Query).Text(" "));

            htmlMessage.Br().Br()
                .Bold("Daftar Catatan:").Br();

            grouped.First(x => x.Key == false)
                .ToList()
                .ForEach(entity => htmlMessage.Text("- ").Code(entity.Query).Br());
        }

        return await _telegramService.SendMessageText(htmlMessage.ToString());
    }
}