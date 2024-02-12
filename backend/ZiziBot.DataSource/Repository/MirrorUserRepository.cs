using MongoFramework.Linq;

namespace ZiziBot.DataSource.Repository;

public class MirrorUserRepository
{
    private readonly MongoDbContextBase _mongoDbContext;

    public MirrorUserRepository(MongoDbContextBase mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }

    public async Task<MirrorUserEntity?> GetByUserId(long userId)
    {
        var userEntity = await _mongoDbContext.MirrorUsers.AsNoTracking()
            .Where(x => x.UserId == userId)
            .Where(x => x.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync();

        return userEntity;
    }
}