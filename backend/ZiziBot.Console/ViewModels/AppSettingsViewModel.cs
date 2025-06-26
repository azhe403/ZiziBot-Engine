using Microsoft.EntityFrameworkCore;
using ZiziBot.Common.Enums;
using ZiziBot.Database;
using ZiziBot.Database.MongoDb.Entities;

namespace ZiziBot.Console.ViewModels;

public class AppSettingsViewModel(DataFacade dataFacade) : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator? Activator { get; }

    [Reactive]
    public IEnumerable<AppSettingsEntity>? AppSettings { get; set; }

    public async Task LoadData()
    {
        AppSettings = await dataFacade.MongoDb.AppSettings.AsNoTracking()
            .Where(w => w.Status == EventStatus.Complete)
            .OrderBy(o => o.Field).ToListAsync();
    }

    public async Task Update(AppSettingsEntity appSettingsEntity)
    {
        dataFacade.MongoDb.AppSettings.Update(appSettingsEntity);
        await dataFacade.MongoDb.SaveChangesAsync();

        await LoadData();
    }
}