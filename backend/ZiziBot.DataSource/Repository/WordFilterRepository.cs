using MongoFramework.Linq;

namespace ZiziBot.DataSource.Repository;

public class WordFilterRepository
{
    private readonly MongoDbContextBase _mongoDbContext;

    public WordFilterRepository(MongoDbContextBase mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }

    public async Task Save(WordFilterEntity entity)
    {
        _mongoDbContext.WordFilter.Add(entity);

        await _mongoDbContext.SaveChangesAsync();
    }

    public async Task<List<WordFilterEntity>> GetAll()
    {
        var data = await _mongoDbContext.WordFilter
            .Where(x => x.Status == (int)EventStatus.Complete)
            .ToListAsync();

        return data;
    }
}