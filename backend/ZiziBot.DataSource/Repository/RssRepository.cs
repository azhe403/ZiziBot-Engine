using Microsoft.EntityFrameworkCore;
using ZiziBot.DataSource.MongoEf;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.DataSource.Repository;

public class RssRepository(MongoEfContext mongoEfContext)
{
    public async Task<RssHistoryEntity?> GetLastRssArticle(long chatId, int threadId, string articleUrl)
    {
        var lastArticle = await mongoEfContext.RssHistory.AsNoTracking()
            .Where(entity => entity.ChatId == chatId)
            .Where(entity => entity.ThreadId == threadId)
            .Where(entity => entity.Url == articleUrl)
            .Where(entity => entity.Status == EventStatus.Complete)
            .FirstOrDefaultAsync();

        return lastArticle;
    }
}