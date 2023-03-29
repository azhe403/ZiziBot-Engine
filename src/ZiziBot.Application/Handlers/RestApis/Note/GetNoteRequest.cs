using System.Net;

namespace ZiziBot.Application.Handlers.RestApis.Note;

public class GetNoteRequestModel : ApiRequestBase<object>
{
}

public class GetNoteRequestHandler : IRequestHandler<GetNoteRequestModel, ApiResponseBase<object>>
{
    private readonly NoteService _noteService;

    public GetNoteRequestHandler(NoteService noteService)
    {
        _noteService = noteService;
    }

    public async Task<ApiResponseBase<object>> Handle(GetNoteRequestModel request, CancellationToken cancellationToken)
    {
        var tags = await _noteService.GetAllByChat(123);

        return new ApiResponseBase<object>()
        {
            StatusCode = HttpStatusCode.OK,
            Result = tags
        };
    }
}