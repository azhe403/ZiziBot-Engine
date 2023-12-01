using MongoFramework.Linq;
using ZiziBot.Contracts.Enums;

namespace ZiziBot.Console.ViewModels;

public class BotUserViewModel : ReactiveObject, IActivatableViewModel
{
    private readonly MongoDbContextBase _mongoDbContextBase;
    public ViewModelActivator Activator { get; }

    [Reactive]
    public IEnumerable<BotUserEntity> BotUsers { get; private set; }

    public BotUserViewModel(MongoDbContextBase mongoDbContextBase)
    {
        _mongoDbContextBase = mongoDbContextBase;

    }

    public async Task GetUsers()
    {
        BotUsers = await _mongoDbContextBase.BotUser.AsNoTracking()
                                            .Where(x => x.Status == (int)EventStatus.Complete)
                                            .ToListAsync();
    }
}