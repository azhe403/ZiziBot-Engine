using MongoFramework.Linq;
using ZiziBot.DataSource.MongoDb;
using ZiziBot.DataSource.MongoDb.Entities;

namespace ZiziBot.DataSource.Repository;

public class WordFilterRepository(
    MongoDbContextBase mongoDbContext,
    ICacheService cacheService
)
{
    public async Task Save(WordFilterEntity entity)
    {
        var wordFilter = await mongoDbContext.WordFilter
            .Where(x => x.Status == (int)EventStatus.Complete)
            .Where(x => x.Word == entity.Word)
            .FirstOrDefaultAsync();

        if (wordFilter == null)
        {
            mongoDbContext.WordFilter.Add(entity);

            await mongoDbContext.SaveChangesAsync();
            await GetAll(true);
        }
    }

    public async Task<List<WordFilterDto>> GetAll(bool evictAfter = false)
    {
        var cache = await cacheService.GetOrSetAsync(
            cacheKey: CacheKey.CHAT_BADWORD,
            evictAfter: evictAfter,
            action: async () => {
                var data = await mongoDbContext.WordFilter
                    .Where(x => x.Status == (int)EventStatus.Complete)
                    .Select(entity => new WordFilterDto() {
                        Id = entity.Id.ToString(),
                        ChatId = entity.ChatId,
                        UserId = entity.UserId,
                        Word = entity.Word,
                        IsGlobal = entity.IsGlobal,
                        CreatedDate = entity.CreatedDate,
                        UpdatedDate = entity.UpdatedDate
                    })
                    .ToListAsync();

                return data;
            });

        return cache;
    }
}