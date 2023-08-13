using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Telegram.Note;

public class FindNoteRequestHandler<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : BotRequestBase, IRequest<TResponse>
    where TResponse : BotResponseBase
{
    private readonly ILogger<FindNoteRequestHandler<TRequest, TResponse>> _logger;
    private readonly TelegramService _telegramService;
    private readonly NoteService _noteService;

    public FindNoteRequestHandler(ILogger<FindNoteRequestHandler<TRequest, TResponse>> logger, TelegramService telegramService, NoteService noteService)
    {
        _logger = logger;
        _telegramService = telegramService;
        _noteService = noteService;
    }

    public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        if (request.Source != ResponseSource.Bot)
        {
            _logger.LogDebug("Find Note stop because Source: {Source}", request.Source);

            return;
        }

        request.ReplyMessage = true;
        request.CleanupTargets = new[]
        {
            CleanupTarget.None
        };

        _telegramService.SetupResponse(request);

        var listNote = await _noteService.GetAllByChat(request.ChatIdentifier);

        if (listNote.Count == 0)
        {
            _logger.LogInformation("No note found in chat {chatId}", request.ChatIdentifier);
            return;
        }

        var words = request.MessageTexts?.Where(x => x.StartsWith('#')).ToList();

        if (words?.Any() ?? false)
        {
            //find note single word - invoked by hashtag (#)
            foreach (var notes in words
                         .Select(word => word.Replace("#", ""))
                         .Select(tag => listNote.FirstOrDefault(x => x.Query == tag))
                         .Where(notes => notes != null)
                    )
                await SendNoteAsync(notes);
        }
        else
        {
            //find note by text - invoked by entire/partial message text
            var note = listNote.FirstOrDefault(x =>
                request.MessageText?.Equals(x.Query, StringComparison.CurrentCultureIgnoreCase) ?? false);

            await SendNoteAsync(note);
        }


        async Task SendNoteAsync(NoteEntity? notes)
        {
            if (notes == null)
            {
                _logger.LogDebug("No Notes for Send to ChatId: {ChatId}", request.ChatId);
                return;
            }

            var dataType = (CommonMediaType)notes.DataType;

            _logger.LogInformation("Sending note {NoteId} with data type {DataType}", notes.Id, dataType);

            var replyMarkup = notes.RawButton.ToButtonMarkup();

            if (dataType <= CommonMediaType.Text)
                await _telegramService.SendMessageText(notes.Content, replyMarkup);
            else
                await _telegramService.SendMediaAsync(
                    notes.FileId,
                    caption: notes.Content,
                    mediaType: (CommonMediaType)notes.DataType,
                    replyMarkup: replyMarkup
                );
        }
    }
}