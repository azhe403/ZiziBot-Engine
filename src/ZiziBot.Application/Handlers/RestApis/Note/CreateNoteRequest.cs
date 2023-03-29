using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.Application.Handlers.RestApis.Note;

public class CreateNoteRequest : ApiRequestBase<bool>
{
    [FromBody]
    public CreateNoteRequestModel Model { get; set; }
}

public class CreateNoteRequestModel
{
    public long ChatId { get; set; }
    public string Slug { get; set; }
    public string Content { get; set; }
    public string? FileId { get; set; }
    public string? RawButton { get; set; }
    public int DataType { get; set; } = -1;
}

public class CreateNoteValidator : AbstractValidator<CreateNoteRequest>
{
    public CreateNoteValidator()
    {
        RuleFor(x => x.Model.ChatId).NotEqual(0).WithMessage("ChatId is required");
        RuleFor(x => x.Model.Slug).NotNull().WithMessage("Slug is required");
    }
}

public class CreateNoteHandler : IRequestHandler<CreateNoteRequest, ApiResponseBase<bool>>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly NoteService _noteService;

    public CreateNoteHandler(IHttpContextAccessor httpContextAccessor, NoteService noteService)
    {
        _httpContextAccessor = httpContextAccessor;
        _noteService = noteService;
    }

    public async Task<ApiResponseBase<bool>> Handle(CreateNoteRequest request, CancellationToken cancellationToken)
    {
        ApiResponseBase<bool> response = new();

        if (!request.ListChatId.Contains(request.Model.ChatId))
        {
            return response.BadRequest("You don't have permission to create note for this Chat");
        }

        await _noteService.Save(new NoteEntity()
        {
            ChatId = request.Model.ChatId,
            Query = request.Model.Slug,
            Content = request.Model.Content,
            FileId = request.Model.FileId,
            RawButton = request.Model.RawButton,
            DataType = request.Model.DataType,
            Status = (int)EventStatus.Complete,
            UserId = _httpContextAccessor.GetUserId(),
            TransactionId = _httpContextAccessor.GetTransactionId()
        });

        return response.Success("Create note success", true);
    }
}