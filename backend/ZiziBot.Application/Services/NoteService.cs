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
            .Where(entity => entity.ChatId == chatId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .OrderBy(entity => entity.Query)
            .ToListAsync();

        return tags;
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

        return result;
    }
}