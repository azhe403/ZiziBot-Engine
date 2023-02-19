using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Services;

public class NoteService
{
    private readonly ILogger<NoteService> _logger;
    private readonly ChatDbContext _chatDbContext;

    public NoteService(ILogger<NoteService> logger, ChatDbContext chatDbContext)
    {
        _logger = logger;
        _chatDbContext = chatDbContext;
    }

    public async Task<List<NoteEntity>> GetAllByChat(long chatId)
    {
        var tags = await _chatDbContext.Note
            .Where(
                entity =>
                    entity.ChatId == chatId &&
                    entity.Status == (int) EventStatus.Complete
            )
            .OrderBy(entity => entity.Query)
            .ToListAsync();

        return tags;
    }

    public async Task Save(NoteEntity entity)
    {
        _chatDbContext.Note.Add(entity);
        await _chatDbContext.SaveChangesAsync();
    }
}