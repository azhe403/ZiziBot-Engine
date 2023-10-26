using MongoFramework.Linq;

namespace ZiziBot.DataSource.Repository;

public class WordFilterRepository
{
    private readonly MongoDbContextBase _mongoDbContext;
    private readonly CacheService _cacheService;

    public WordFilterRepository(MongoDbContextBase mongoDbContext, CacheService cacheService)
    {
        _mongoDbContext = mongoDbContext;
        _cacheService = cacheService;
    }

    public async Task Save(WordFilterEntity entity)
    {
        _mongoDbContext.WordFilter.Add(entity);

        await _mongoDbContext.SaveChangesAsync();
    }

    public async Task<List<WordFilterDto>> GetAll()
    {
        var cache = await _cacheService.GetOrSetAsync(
            cacheKey: CacheKey.BADWORD,
            action: async () => {
                var data = await _mongoDbContext.WordFilter
                    .Where(x => x.Status == (int)EventStatus.Complete)
                    .Select(entity => new WordFilterDto()
                    {
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