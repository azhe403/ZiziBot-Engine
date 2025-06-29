using Microsoft.EntityFrameworkCore;
using ZiziBot.Common.Enums;
using ZiziBot.Database;
using ZiziBot.Database.MongoDb.Entities;

namespace ZiziBot.Console.ViewModels;

public class BotUserViewModel(DataFacade dataFacade) : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }

    [Reactive]
    public IEnumerable<BotUserEntity> BotUsers { get; private set; }

    public async Task GetUsers()
    {
        BotUsers = await dataFacade.MongoDb.BotUser.AsNoTracking()
            .Where(x => x.Status == EventStatus.Complete)
            .ToListAsync();
    }
}