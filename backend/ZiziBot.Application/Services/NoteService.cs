using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Services;

public class NoteService
{
    private readonly ILogger<NoteService> _logger;
    private readonly ChatDbContext _chatDbContext;
    private readonly CacheService _cacheService;

    public NoteService(ILogger<NoteService> logger, ChatDbContext chatDbContext, CacheService cacheService)
    {
        _logger = logger;
        _chatDbContext = chatDbContext;
        _cacheService = cacheService;
    }

    public async Task<List<NoteEntity>> GetAllByChat(long chatId, bool evictBefore = false)
    {
        var cache = await _cacheService.GetOrSetAsync(
            cacheKey: $"notes/{chatId}",
            evictBefore: evictBefore,
            action: async () => {
                var tags = await _chatDbContext.Note
                    .Where(entity => entity.ChatId == chatId)
                    .Where(entity => entity.Status == (int)EventStatus.Complete)
                    .OrderBy(entity => entity.Query)
                    .ToListAsync();

                return tags;
            });

        return cache;
    }

    public async Task<ServiceResult> Save(NoteEntity entity)
    {
        ServiceResult result = new();
        _logger.LogInformation("Checking Note with Query: {Query}", entity.Query);

        var findNote = await _chatDbContext.Note
            .Where(x => x.Id == entity.Id)
            .Where(x => x.ChatId == entity.ChatId)
            .FirstOrDefaultAsync();

        if (findNote == null)
        {
            _logger.LogInformation("Adding Note with Query: {Query}", entity.Query);
            _chatDbContext.Note.Add(entity);

            result.Message = "Note created successfully";
        }
        else
        {
            _logger.LogInformation("Updating Note with Id: {Id}", entity.Id);

            findNote.Query = entity.Query;
            findNote.Content = entity.Content;
            findNote.DataType = entity.DataType;
            findNote.FileId = entity.FileId;
            findNote.RawButton = entity.RawButton;
            findNote.TransactionId = entity.TransactionId;
            findNote.UserId = entity.UserId;

            result.Message = "Note updated successfully";
        }

        await _chatDbContext.SaveChangesAsync();

        await GetAllByChat(entity.ChatId, true);

        return result;
    }

    public async Task<BotResponseBase> Delete(long chatId, string note, Func<string, Task<BotResponseBase>> func)
    {
        var findNote = await _chatDbContext.Note
            .Where(x => x.ChatId == chatId)
            .Where(x => x.Query == note)
            .Where(x => x.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync();

        if (findNote == null)
            return await func.Invoke("Note tidak ditemukan, mungkin sudah dihapus.");

        findNote.Status = (int)EventStatus.Deleted;

        await _chatDbContext.SaveChangesAsync();

        return await func.Invoke("Note berhasil dihapus");
    }
}