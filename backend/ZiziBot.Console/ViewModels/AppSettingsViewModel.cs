using MongoFramework.Linq;
using ZiziBot.Contracts.Enums;

namespace ZiziBot.Console.ViewModels;

public class AppSettingsViewModel : ReactiveObject, IActivatableViewModel
{
    private readonly MongoDbContextBase _mongoDbContextBase;
    public ViewModelActivator? Activator { get; }

    [Reactive]
    public IEnumerable<AppSettingsEntity>? AppSettings { get; set; }

    public AppSettingsViewModel(MongoDbContextBase mongoDbContextBase)
    {
        _mongoDbContextBase = mongoDbContextBase;
    }

    public async Task LoadData()
    {
        AppSettings = await _mongoDbContextBase.AppSettings.AsNoTracking()
            .Where(w => w.Status == (int)EventStatus.Complete)
            .OrderBy(o => o.Field).ToListAsync();
    }

    public async Task Update(AppSettingsEntity appSettingsEntity)
    {
        _mongoDbContextBase.AppSettings.Update(appSettingsEntity);
        await _mongoDbContextBase.SaveChangesAsync();

        await LoadData();
    }
}