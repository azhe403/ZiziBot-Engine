using MongoFramework.Linq;

namespace ZiziBot.Application.Services;

public class SudoService
{
    private readonly AppSettingsDbContext _appSettingsDbContext;
    private readonly CacheService _cacheService;

    public SudoService(AppSettingsDbContext appSettingsDbContext, CacheService cacheService)
    {
        _appSettingsDbContext = appSettingsDbContext;
        _cacheService = cacheService;
    }

    public async Task<bool> IsSudoAsync(long userId)
    {
        var cache = await _cacheService.GetOrSetAsync(
            cacheKey: CacheKey.SUDO + userId,
            action: async () => {
                return await _appSettingsDbContext.Sudoers.AnyAsync(
                    x =>
                        x.UserId == userId &&
                        x.Status == (int) EventStatus.Complete
                );
            }
        );

        return cache;
    }

    public async Task<ServiceResult> SaveSudo(Sudoer sudoer)
    {
        ServiceResult serviceResult = new();

        var findSudo = await _appSettingsDbContext.Sudoers
            .FirstOrDefaultAsync(x => x.UserId == sudoer.UserId);

        if (findSudo != null)
        {
            return serviceResult.Complete("This user is already a sudoer.");
        }

        _appSettingsDbContext.Sudoers.Add(sudoer);
        await _appSettingsDbContext.SaveChangesAsync();

        return serviceResult.Complete("Sudoer added successfully.");
    }
}