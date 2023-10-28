using ZiziBot.DataSource.MongoDb.Entities;

namespace ZiziBot.Console.ViewModels;

public class BotUserViewModel : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }

    [Reactive]
    private IEnumerable<BotUserEntity> BotUser { get; set; }

}