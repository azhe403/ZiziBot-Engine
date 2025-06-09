using Microsoft.EntityFrameworkCore;
using ZiziBot.DataSource.MongoEf;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.DataSource.Repository;

public class MirrorUserRepository(
    MongoEfContext mongoEfContext
)
{
    public async Task<MirrorUserEntity?> GetByUserId(long userId)
    {
        var userEntity = await mongoEfContext.MirrorUser.AsNoTracking()
            .Where(x => x.UserId == userId)
            .FirstOrDefaultAsync();

        return userEntity;
    }

    public async Task<int> SaveActivity(MirrorActivityDto dto)
    {
        await mongoEfContext.MirrorActivity.AddAsync(new() {
            UserId = dto.UserId,
            ActivityTypeId = (int)dto.ActivityTypeId,
            ActivityName = dto.ActivityTypeId.ToString(),
            Url = dto.Url,
            Status = EventStatus.Complete,
            TransactionId = dto.TransactionId,
            CreatedDate = default,
            UpdatedDate = default
        });

        return await mongoEfContext.SaveChangesAsync();
    }

    public async Task<MirrorDonationEntity?> GetDonation(string orderId)
    {
        var mirrorDonationEntity = await mongoEfContext.MirrorDonation.AsNoTracking()
            .Where(x => x.OrderId == orderId)
            .Where(x => x.Status == EventStatus.Complete)
            .FirstOrDefaultAsync();

        return mirrorDonationEntity;
    }
}