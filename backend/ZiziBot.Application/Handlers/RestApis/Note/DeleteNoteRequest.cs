using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace ZiziBot.Application.Handlers.RestApis.Note;

public class DeleteNoteRequest : ApiRequestBase<bool>
{
    [FromBody]
    public DeleteNoteRequestBody Body { get; set; }
}

public class DeleteNoteRequestBody
{
    public long ChatId { get; set; }
    public string Id { get; set; }
}

public class DeleteNoteHandler(
    DataFacade dataFacade
) : IApiRequestHandler<DeleteNoteRequest, bool>
{
    public async Task<ApiResponseBase<bool>> Handle(DeleteNoteRequest request, CancellationToken cancellationToken)
    {
        ApiResponseBase<bool> response = new();

        var note = await dataFacade.MongoEf.Note
            .Where(entity => entity.ChatId == request.Body.ChatId)
            .Where(entity => entity.Id == new ObjectId(request.Body.Id))
            .Where(entity => entity.Status == EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (note == null)
        {
            return response.BadRequest("Note not found");
        }

        note.Status = EventStatus.Deleted;

        await dataFacade.MongoEf.SaveChangesAsync(cancellationToken);

        return response.Success("Note deleted successfully.");
    }
}