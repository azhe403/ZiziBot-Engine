using MongoDB.Bson;
using MongoFramework.Linq;

namespace ZiziBot.DataSource.Repository;

public class GroupRepository
{
    private readonly ChatDbContext _chatDbContext;
    private readonly GroupDbContext _groupDbContext;

    public GroupRepository(ChatDbContext chatDbContext, GroupDbContext groupDbContext)
    {
        _chatDbContext = chatDbContext;
        _groupDbContext = groupDbContext;
    }

    public async Task<WelcomeMessageDto?> GetWelcomeMessageById(string welcomeId)
    {
        var query = await _groupDbContext.WelcomeMessage
            .AsNoTracking()
            .Where(entity => entity.Id == new ObjectId(welcomeId))
            .FirstOrDefaultAsync();

        if (query == null)
            return default;

        var listChatSetting = await _chatDbContext.ChatSetting
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
}