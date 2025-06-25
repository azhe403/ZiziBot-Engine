using Microsoft.EntityFrameworkCore;
using ZiziBot.Application.Facades;
using ZiziBot.Common.Enums;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.Console.ViewModels;

public class AppSettingsViewModel(DataFacade dataFacade) : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator? Activator { get; }

    [Reactive]
    public IEnumerable<AppSettingsEntity>? AppSettings { get; set; }

    public async Task LoadData()
    {
        AppSettings = await dataFacade.MongoEf.AppSettings.AsNoTracking()
            .Where(w => w.Status == EventStatus.Complete)
            .OrderBy(o => o.Field).ToListAsync();
    }

    public async Task Update(AppSettingsEntity appSettingsEntity)
    {
        dataFacade.MongoEf.AppSettings.Update(appSettingsEntity);
        await dataFacade.MongoEf.SaveChangesAsync();

        await LoadData();
    }
}