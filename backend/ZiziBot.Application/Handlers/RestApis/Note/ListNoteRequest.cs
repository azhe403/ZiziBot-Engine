namespace ZiziBot.Application.Handlers.RestApis.Note;

public class ListNoteRequest : ApiRequestBase<List<NoteDto>>
{
    public long ChatId { get; set; }
}

public class ListNoteHandler : IRequestHandler<ListNoteRequest, ApiResponseBase<List<NoteDto>>>
{
    private readonly ChatSettingRepository _chatSettingRepository;

    public ListNoteHandler(ChatSettingRepository chatSettingRepository)
    {
        _chatSettingRepository = chatSettingRepository;
    }

    public async Task<ApiResponseBase<List<NoteDto>>> Handle(ListNoteRequest request, CancellationToken cancellationToken)
    {
        var response = new ApiResponseBase<List<NoteDto>>();

        if (!request.ListChatId.Contains(request.ChatId))
        {
            return response.BadRequest("ChatId is not in your list");
        }

        var listNote = await _chatSettingRepository.GetListNote(request.ChatId);

        return response.Success("Get Note successfully", listNote);
    }
}