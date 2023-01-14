using MongoFramework.Linq;

namespace ZiziBot.Application.Services;

public class SudoService
{
    private readonly AppSettingsDbContext _appSettingsDbContext;

    public SudoService(AppSettingsDbContext appSettingsDbContext)
    {
        _appSettingsDbContext = appSettingsDbContext;
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