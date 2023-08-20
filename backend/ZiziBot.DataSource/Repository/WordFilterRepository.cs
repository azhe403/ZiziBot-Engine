using MongoFramework.Linq;

namespace ZiziBot.DataSource.Repository;

public class WordFilterRepository
{
    private readonly ChatDbContext _chatDbContext;
    private readonly MongoDbContextBase _mongoDbContext;

    public WordFilterRepository(ChatDbContext chatDbContext, MongoDbContextBase mongoDbContext)
    {
        _chatDbContext = chatDbContext;
        _mongoDbContext = mongoDbContext;
    }

    public async Task Save(WordFilterEntity entity)
    {
        _chatDbContext.WordFilter.Add(entity);

        await _chatDbContext.SaveChangesAsync();
    }

    public async Task<List<WordFilterEntity>> GetAll()
    {
        var data = await _chatDbContext.WordFilter
            .Where(x => x.Status == (int)EventStatus.Complete)
            .ToListAsync();

        return data;
    }
}