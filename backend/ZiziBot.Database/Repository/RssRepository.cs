using Microsoft.EntityFrameworkCore;
using ZiziBot.Database.MongoDb;
using ZiziBot.Database.MongoDb.Entities;

namespace ZiziBot.Database.Repository;

public class RssRepository(MongoDbContext mongoDbContext)
{
    public async Task<RssHistoryEntity?> GetLastRssArticle(long chatId, int threadId, string articleUrl)
    {
        var lastArticle = await mongoDbContext.RssHistory.AsNoTracking()
            .Where(entity => entity.ChatId == chatId)
            .Where(entity => entity.ThreadId == threadId)
            .Where(entity => entity.Url == articleUrl)
            .Where(entity => entity.Status == EventStatus.Complete)
            .FirstOrDefaultAsync();

        return lastArticle;
    }
}