using MongoFramework.Linq;
using ZiziBot.Contracts.Enums;
using ZiziBot.Types.Types;
using Unit = System.Reactive.Unit;

namespace ZiziBot.Console.ViewModels;

public class WebhookViewModel : ReactiveObject
{
    private readonly MongoDbContextBase _mongoDbContextBase;
    private readonly ObservableAsPropertyHelper<List<WebhookChatEntity>> _listWebhookChat;

    [Reactive]
    public LoadingConfiguration Loading { get; set; } = new();

    public List<WebhookChatEntity> ListWebhookChat => _listWebhookChat.Value;

    public ReactiveCommand<Unit, List<WebhookChatEntity>> LoadDataCommand { get; }

    public WebhookViewModel(MongoDbContextBase mongoDbContextBase)
    {
        _mongoDbContextBase = mongoDbContextBase;

        Loading.TotalSteps = 2;

        LoadDataCommand = ReactiveCommand.CreateFromTask(LoadData);

        _listWebhookChat =
            LoadDataCommand.ToProperty(this, x => x.ListWebhookChat, scheduler: RxApp.MainThreadScheduler);
    }

    private async Task<List<WebhookChatEntity>> LoadData()
    {
        Loading.CurrentStep = 1;

        var data = await _mongoDbContextBase.WebhookChat
            .Where(x => x.Status == (int)EventStatus.Complete)
            .ToListAsync();

        Loading.CurrentStep = 2;

        return data;
    }
}