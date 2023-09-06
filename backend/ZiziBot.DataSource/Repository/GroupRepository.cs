using MongoDB.Bson;
using MongoFramework.Linq;

namespace ZiziBot.DataSource.Repository;

public class GroupRepository
{
    private readonly MongoDbContextBase _mongoDbContext;

    public GroupRepository(MongoDbContextBase mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }

    public async Task<WelcomeMessageDto?> GetWelcomeMessageById(string welcomeId)
    {
        var query = await _mongoDbContext.WelcomeMessage
            .AsNoTracking()
            .Where(entity => entity.Id == new ObjectId(welcomeId))
            .FirstOrDefaultAsync();

        if (query == null)
            return default;

        var listChatSetting = await _mongoDbContext.ChatSetting
            .Where(entity => entity.ChatId == query.ChatId)
            .FirstOrDefaultAsync();

        if (listChatSetting == null)
            return default;

        var data = new WelcomeMessageDto()
        {
            Id = query.Id.ToString(),
            ChatId = query.ChatId,
            ChatTitle = listChatSetting.ChatTitle,
            Text = query.Text,
            RawButton = query.RawButton,
            Media = query.Media,
            DataType = query.DataType,
            DataTypeName = ((CommonMediaType)query.DataType).ToString(),
            Status = query.Status,
            StatusName = ((EventStatus)query.Status).ToString()
        };

        return data;
    }

    public async Task<List<ChatAdminEntity>> GetChatAdminByUserId(long userId)
    {
        var listChatAdmin = await _mongoDbContext.ChatAdmin
            .Where(entity => entity.UserId == userId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .ToListAsync();

        return listChatAdmin;
    }
}