using Microsoft.AspNetCore.Mvc;

namespace ZiziBot.Application.Handlers.RestApis.Note;

public class GetNoteRequest : ApiRequestBase<object>
{
    [FromRoute]
    public string NoteId { get; set; }
}

public class GetNoteHandler : IRequestHandler<GetNoteRequest, ApiResponseBase<object>>
{
    private readonly ChatSettingRepository _chatSettingRepository;
    private readonly ChatDbContext _chatDbContext;

    public GetNoteHandler(ChatSettingRepository chatSettingRepository)
    {
        _chatSettingRepository = chatSettingRepository;
    }

    public async Task<ApiResponseBase<object>> Handle(GetNoteRequest request, CancellationToken cancellationToken)
    {
        var response = new ApiResponseBase<object>();

        var notes = await _chatSettingRepository.GetNote(request.NoteId);

        return response.Success("Get detail Note successfully", notes);
    }
}