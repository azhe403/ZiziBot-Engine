using FluentValidation;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Note;

public class CreateNoteRequestModel : RequestBase
{
    public string Query { get; set; }
    public string? Content { get; set; }
    public string? FileId { get; set; }
    public string? RawButton { get; set; }
    public int DataType { get; set; }
    public bool? RefreshNote { get; set; }
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
    private readonly ChatDbContext _chatDbContext;

    public CreateNoteRequestHandler(TelegramService telegramService, ChatDbContext chatDbContext)
    {
        _telegramService = telegramService;
        _chatDbContext = chatDbContext;
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

        var note = await _chatDbContext.Note
            .FirstOrDefaultAsync(entity =>
                    entity.ChatId == request.ChatIdentifier &&
                    entity.Query == request.Query &&
                    entity.Status == (int) EventStatus.Complete,
                cancellationToken: cancellationToken
            );

        if (note != null)
        {
            if (request.RefreshNote ?? false)
            {
                await _telegramService.SendMessageText("Sedang memperbarui catatan...");

                note.Content = request.Content;
                note.FileId = request.FileId;
                note.RawButton = request.RawButton;
                note.DataType = request.DataType;
                note.Status = (int) EventStatus.Complete;
            }
            else
            {
                htmlMessage.Text("Catatan sudah ada, silahkan gunakan nama lainnya. Atau gunakan perintah /renote untuk memperbarui catatan");
                return await _telegramService.SendMessageText(htmlMessage.ToString());
            }
        }
        else
        {
            await _telegramService.SendMessageText("Sedang membuat catatan...");

            _chatDbContext.Note.Add(new NoteEntity()
            {
                ChatId = request.ChatIdentifier,
                UserId = request.UserId,
                Query = request.Query,
                Content = request.Content,
                FileId = request.FileId,
                RawButton = request.RawButton,
                DataType = request.DataType,
                Status = (int) EventStatus.Complete
            });
        }

        await _chatDbContext.SaveChangesAsync(cancellationToken);

        return await _telegramService.EditMessageText("Catatan berhasil disimpan");
    }
}