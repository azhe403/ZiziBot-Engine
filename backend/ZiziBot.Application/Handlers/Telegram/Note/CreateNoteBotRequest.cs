using FluentValidation;
using MongoFramework.Linq;
using ZiziBot.DataSource.MongoDb.Entities;

namespace ZiziBot.Application.Handlers.Telegram.Note;

public class CreateNoteBotRequest : BotRequestBase
{
    public string Query { get; set; }
    public string? Content { get; set; }
    public string? FileId { get; set; }
    public string? RawButton { get; set; }
    public int DataType { get; set; }
    public bool? RefreshNote { get; set; }
}

public class CreateNoteValidator : AbstractValidator<CreateNoteBotRequest>
{
    public CreateNoteValidator()
    {
        RuleFor(x => x.Query).NotNull().When(x => x.Content != null).WithMessage("Masukkan Query untuk Note nya");
        RuleFor(x => x.Query).MinimumLength(2).WithMessage("Panjang Query minimal 2 karakter");
        RuleFor(x => x.Content).NotNull().When(x => x.FileId == null)
            .WithMessage("Balas sebuah pesan yang mengandung Teks");
    }
}

public class CreateNoteHandler(
    TelegramService telegramService,
    NoteService noteService,
    MongoDbContextBase mongoDbContext)
    : IRequestHandler<CreateNoteBotRequest, BotResponseBase>
{
    public async Task<BotResponseBase> Handle(CreateNoteBotRequest request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        var htmlMessage = HtmlMessage.Empty;

        var validationResult = await request.ValidateAsync<CreateNoteValidator, CreateNoteBotRequest>();

        if (!validationResult.IsValid)
        {
            htmlMessage.Text(validationResult.Errors.Select(x => x.ErrorMessage).Aggregate((x, y) => $"{x})\n{y}"));
            return await telegramService.SendMessageText(htmlMessage.ToString());
        }

        if (request.ReplyToMessage == null)
        {
            await telegramService.SendMessageText("Balas sebuah pesan yang akan disimpan");
        }

        var note = await mongoDbContext.Note
            .FirstOrDefaultAsync(entity =>
                    entity.ChatId == request.ChatIdentifier &&
                    entity.Query == request.Query &&
                    entity.Status == (int)EventStatus.Complete,
                cancellationToken: cancellationToken
            );

        var btnFromMessage = request.Message.GetRawReplyMarkup();

        if (request.RawButton.IsNullOrEmpty())
        {
            request.RawButton = btnFromMessage;
        }

        if (note != null)
        {
            if (request.RefreshNote ?? false)
            {
                await telegramService.SendMessageText("Sedang memperbarui catatan...");

                note.Content = request.Content;
                note.FileId = request.FileId;
                note.RawButton = request.RawButton;
                note.DataType = request.DataType;
                note.Status = (int)EventStatus.Complete;
                note.TransactionId = request.TransactionId;
            }
            else
            {
                htmlMessage.Text(
                    "Catatan sudah ada, silahkan gunakan nama lainnya. Atau gunakan perintah /renote untuk memperbarui catatan");
                return await telegramService.SendMessageText(htmlMessage.ToString());
            }
        }
        else
        {
            await telegramService.SendMessageText("Sedang membuat catatan...");

            mongoDbContext.Note.Add(new NoteEntity() {
                ChatId = request.ChatIdentifier,
                UserId = request.UserId,
                Query = request.Query,
                Content = request.Content,
                FileId = request.FileId,
                RawButton = request.RawButton,
                DataType = request.DataType,
                Status = (int)EventStatus.Complete,
                TransactionId = request.TransactionId
            });
        }

        await mongoDbContext.SaveChangesAsync(cancellationToken);

        await telegramService.EditMessageText("Memperbarui cache..");
        await noteService.GetAllByChat(request.ChatIdentifier, true);

        return await telegramService.EditMessageText("Catatan berhasil disimpan");
    }
}