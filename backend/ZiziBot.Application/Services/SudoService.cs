using MongoFramework.Linq;
using ZiziBot.DataSource.MongoDb.Entities;

namespace ZiziBot.Application.Services;

public class SudoService
{
    private readonly MongoDbContextBase _mongoDbContext;
    private readonly CacheService _cacheService;

    public SudoService(MongoDbContextBase mongoDbContext, CacheService cacheService)
    {
        _mongoDbContext = mongoDbContext;
        _cacheService = cacheService;
    }

    public async Task<bool> IsSudoAsync(long userId)
    {
        var cache = await _cacheService.GetOrSetAsync(
            cacheKey: CacheKey.SUDO + userId,
            action: async () => {
                return await _mongoDbContext.Sudoers.AnyAsync(
                    x =>
                        x.UserId == userId &&
                        x.Status == (int)EventStatus.Complete
                );
            }
        );

        return cache;
    }

    public async Task<ServiceResult> SaveSudo(SudoerEntity entity)
    {
        ServiceResult serviceResult = new();

        var findSudo = await _mongoDbContext.Sudoers
            .FirstOrDefaultAsync(x => x.UserId == entity.UserId);

        if (findSudo != null)
        {
            return serviceResult.Complete("This user is already a sudoer.");
        }

        _mongoDbContext.Sudoers.Add(entity);
        await _mongoDbContext.SaveChangesAsync();

        return serviceResult.Complete("Sudoer added successfully.");
    }
}