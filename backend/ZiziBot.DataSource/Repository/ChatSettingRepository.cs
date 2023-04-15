using MongoFramework.Linq;

namespace ZiziBot.DataSource.Repository;

public class ChatSettingRepository
{
    private readonly ChatDbContext _chatDbContext;

    public ChatSettingRepository(ChatDbContext chatDbContext)
    {
        _chatDbContext = chatDbContext;
    }

    public async Task<WebhookChatEntity?> GetWebhookRouteById(string routeId)
    {
        var webhookChat = await _chatDbContext.WebhookChat
            .Where(entity => entity.RouteId == routeId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync();

        return webhookChat;
    }
}