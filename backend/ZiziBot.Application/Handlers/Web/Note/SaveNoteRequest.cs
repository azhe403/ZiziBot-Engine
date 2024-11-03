using FluentValidation;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;

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

public class CreateNoteHandler(IHttpContextAccessor httpContextAccessor, NoteService noteService) : IWebRequestHandler<SaveNoteRequest, bool>
{
    public async Task<WebResponseBase<bool>> Handle(SaveNoteRequest request, CancellationToken cancellationToken)
    {
        WebResponseBase<bool> response = new();

        var save = await noteService.Save(new() {
            Id = request.ObjectId,
            ChatId = request.ChatId,
            Query = request.Query,
            Content = request.Content,
            FileId = request.FileId,
            RawButton = request.RawButton,
            DataType = request.DataType,
            Status = EventStatus.Complete,
            UserId = httpContextAccessor.GetUserId(),
            TransactionId = httpContextAccessor.GetTransactionId()
        });

        return response.Success(save.Message, true);
    }
}