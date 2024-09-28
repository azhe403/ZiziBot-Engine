namespace ZiziBot.Application.Handlers.Telegram.Note;

public class GetNoteBotRequestModel : BotRequestBase
{
}

public class GetNoteRequestHandler(TelegramService telegramService, NoteService noteService) : IRequestHandler<GetNoteBotRequestModel, BotResponseBase>
{
    public async Task<BotResponseBase> Handle(GetNoteBotRequestModel request, CancellationToken cancellationToken)
    {
        var htmlMessage = HtmlMessage.Empty;
        telegramService.SetupResponse(request);

        var allNotes = await noteService.GetAllByChat(request.ChatIdentifier);

        if (allNotes.Count == 0)
        {
            htmlMessage.Text("Tidak ada catatan di Obrolan ini.");
        }
        else
        {
            var grouped = allNotes
                .GroupBy(entity => entity.Query.Split(" ").Length <= 1)
                .ToList();

            var tags = grouped.FirstOrDefault(x => x.Key == true);
            if (tags != null)
            {
                htmlMessage.Bold("Daftar Tagar:").Br();
                tags.ToList()
                    .ForEach(entity => htmlMessage.Text("#").Text(entity.Query).Text(" "));
            }

            var notes = grouped.FirstOrDefault(x => x.Key == false);
            if (notes != null)
            {
                htmlMessage.Br().Br()
                    .Bold("Daftar Catatan:").Br();

                notes
                    .ToList()
                    .ForEach(entity => htmlMessage.Text("- ").Code(entity.Query).Br());
            }
        }

        return await telegramService.SendMessageText(htmlMessage.ToString());
    }
}