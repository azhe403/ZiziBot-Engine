using FluentValidation;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using ZiziBot.DataSource.MongoDb.Entities;

namespace ZiziBot.Application.Handlers.Web.Note;

public class SaveNoteRequest : WebRequestBase<bool>
{
    public string? Id { get; set; }
    public long ChatId { get; set; }
    public string Query { get; set; }
    public string Content { get; set; }
    public string? FileId { get; set; }
    public string? RawButton { get; set; }
    public int DataType { get; set; } = -1;
    public ObjectId ObjectId => Id != null ? new ObjectId(Id) : ObjectId.Empty;
}

public class SaveNoteValidator : AbstractValidator<SaveNoteRequest>
{
    public SaveNoteValidator()
    {
        RuleFor(x => x.ChatId).NotEqual(0).WithMessage("ChatId is required");
        RuleFor(x => x.Query).NotNull().WithMessage("Slug is required");
    }
}

public class CreateNoteHandler : IRequestHandler<SaveNoteRequest, WebResponseBase<bool>>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly NoteService _noteService;

    public CreateNoteHandler(IHttpContextAccessor httpContextAccessor, NoteService noteService)
    {
        _httpContextAccessor = httpContextAccessor;
        _noteService = noteService;
    }

    public async Task<WebResponseBase<bool>> Handle(SaveNoteRequest request, CancellationToken cancellationToken)
    {
        WebResponseBase<bool> response = new();

        var save = await _noteService.Save(new NoteEntity() {
            Id = request.ObjectId,
            ChatId = request.ChatId,
            Query = request.Query,
            Content = request.Content,
            FileId = request.FileId,
            RawButton = request.RawButton,
            DataType = request.DataType,
            Status = (int)EventStatus.Complete,
            UserId = _httpContextAccessor.GetUserId(),
            TransactionId = _httpContextAccessor.GetTransactionId()
        });

        return response.Success(save.Message, true);
    }
}