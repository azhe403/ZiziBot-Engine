using FluentValidation;

namespace ZiziBot.Application.Handlers.Telegram.Note;

public class CreateNoteRequestModel : RequestBase
{
    public string Query { get; set; }
    public string? Content { get; set; }
    public string? FileId { get; set; }
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
    private readonly NoteService _noteService;

    public CreateNoteRequestHandler(NoteService noteService)
    {
        _noteService = noteService;
    }

    public async Task<ResponseBase> Handle(CreateNoteRequestModel request, CancellationToken cancellationToken)
    {
        ResponseBase responseBase = new(request);

        var htmlMessage = HtmlMessage.Empty;

        var validationResult = await request.ValidateAsync<CreateNoteValidator, CreateNoteRequestModel>();

        if ((!validationResult?.IsValid) ?? false)
        {
            htmlMessage.Text(validationResult.Errors.Select(x => x.ErrorMessage).Aggregate((x, y) => $"{x})\n{y}"));
            return await responseBase.SendMessageText(htmlMessage.ToString());
        }

        await responseBase.SendMessageText("Sedang membuat catatan...");

        await _noteService.Save(
            new NoteEntity()
            {
                Query = request.Query,
                Content = request.Content,
                ChatId = request.ChatIdentifier,
                FileId = request.FileId,
                Status = (int) EventStatus.Complete,
                UserId = request.UserId
            }
        );

        return await responseBase.EditMessageText("Catatan berhasil dibuat");
    }
}