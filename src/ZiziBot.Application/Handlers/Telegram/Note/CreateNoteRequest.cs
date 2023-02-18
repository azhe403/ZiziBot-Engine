using FluentValidation;

namespace ZiziBot.Application.Handlers.Telegram.Note;

public class CreateNoteRequestModel : RequestBase
{
    public string Query { get; set; }
    public string? Content { get; set; }
    public string? FileId { get; set; }
    public string? RawButton { get; set; }
}

public class CreateNoteValidator : AbstractValidator<CreateNoteRequestModel>
{
    public CreateNoteValidator()
    {
        RuleFor(model => model.Query).NotNull().MinimumLength(1).WithMessage("Query minimal 3 karakter");
    }
}

public class CreateNoteRequestHandler : IRequestHandler<CreateNoteRequestModel, ResponseBase>
{
    private readonly TelegramService _telegramService;
    private readonly NoteService _noteService;

    public CreateNoteRequestHandler(TelegramService telegramService, NoteService noteService)
    {
        _telegramService = telegramService;
        _noteService = noteService;
    }

    public async Task<ResponseBase> Handle(CreateNoteRequestModel request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        var htmlMessage = HtmlMessage.Empty;

        var validationResult = await request.ValidateAsync<CreateNoteValidator, CreateNoteRequestModel>();

        if ((!validationResult?.IsValid) ?? false)
        {
            htmlMessage.Text(validationResult.Errors.Select(x => x.ErrorMessage).Aggregate((x, y) => $"{x})\n{y}"));
            return await _telegramService.SendMessageText(htmlMessage.ToString());
        }

        await _telegramService.SendMessageText("Sedang membuat catatan...");

        await _noteService.Save(
            new NoteEntity()
            {
                ChatId = request.ChatIdentifier,
                UserId = request.UserId,
                Query = request.Query,
                Content = request.Content,
                FileId = request.FileId,
                RawButton = request.RawButton,
                DataType = (int) request.ReplyToMessage.Type,
                Status = (int) EventStatus.Complete,
            }
        );

        return await _telegramService.EditMessageText("Catatan berhasil dibuat");
    }
}