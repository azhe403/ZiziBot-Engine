using MongoFramework.Linq;
using ZiziBot.DataSource.MongoDb;
using ZiziBot.DataSource.MongoDb.Entities;
using ZiziBot.DataSource.MongoEf;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.DataSource.Repository;

public class MirrorUserRepository(
    MongoDbContextBase mongoDbContext,
    MongoEfContext mongoEfContext
)
{
    public async Task<MirrorUserEntity?> GetByUserId(long userId)
    {
        var userEntity = await mongoDbContext.MirrorUsers.AsNoTracking()
            .Where(x => x.UserId == userId)
            .FirstOrDefaultAsync();

        return userEntity;
    }

    public async Task<int> SaveActivity(MirrorActivityDto dto)
    {
        await mongoEfContext.MirrorActivity.AddAsync(new MirrorActivityEntity {
            UserId = dto.UserId,
            ActivityTypeId = (int)dto.ActivityTypeId,
            ActivityName = dto.ActivityTypeId.ToString(),
            Url = dto.Url,
            Status = EventStatus.Complete,
            TransactionId = dto.TransactionId,
            CreatedDate = default,
            UpdatedDate = default,
        });

        return await mongoEfContext.SaveChangesAsync();
    }
}