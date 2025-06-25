using Microsoft.EntityFrameworkCore;
using ZiziBot.Application.Facades;
using ZiziBot.Common.Enums;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.Console.ViewModels;

public class BotUserViewModel(DataFacade dataFacade) : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }

    [Reactive]
    public IEnumerable<BotUserEntity> BotUsers { get; private set; }

    public async Task GetUsers()
    {
        BotUsers = await dataFacade.MongoEf.BotUser.AsNoTracking()
            .Where(x => x.Status == EventStatus.Complete)
            .ToListAsync();
    }
}