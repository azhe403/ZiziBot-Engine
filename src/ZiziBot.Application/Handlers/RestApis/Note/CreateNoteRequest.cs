using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace ZiziBot.Application.Handlers.RestApis.Note;

public class CreateNoteRequestModel : ApiRequestBase<bool>
{
    public long ChatId { get; set; }
    public string Slug { get; set; }
    public string Content { get; set; }
    public string FileId { get; set; }
}

public class CreateNoteValidator : AbstractValidator<CreateNoteRequestModel>
{
    public CreateNoteValidator()
    {
        RuleFor(model => model.Slug).NotNull().MinimumLength(1);
    }
}

public class CreateNoteRequestHandler : IRequestHandler<CreateNoteRequestModel, ApiResponseBase<bool>>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly NoteService _noteService;

    public CreateNoteRequestHandler(IHttpContextAccessor httpContextAccessor, NoteService noteService)
    {
        _httpContextAccessor = httpContextAccessor;
        _noteService = noteService;
    }

    public async Task<ApiResponseBase<bool>> Handle(CreateNoteRequestModel request, CancellationToken cancellationToken)
    {
        ApiResponseBase<bool> responseBase = new();

        await _noteService.Save(
            new NoteEntity()
            {
                ChatId = request.ChatId,
                Query = request.Slug,
                Content = request.Content,
                FileId = request.FileId,
                Status = (int)EventStatus.Complete,
                UserId = _httpContextAccessor.GetUserId(),
                TransactionId = _httpContextAccessor.GetTransactionId()
            }
        );

        responseBase.StatusCode = HttpStatusCode.OK;
        responseBase.Result = true;

        return responseBase;
    }
}