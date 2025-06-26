using Microsoft.EntityFrameworkCore;
using ZiziBot.Common.Enums;
using ZiziBot.Common.Types;
using ZiziBot.Database;
using ZiziBot.Database.MongoDb.Entities;
using Unit = System.Reactive.Unit;

namespace ZiziBot.Console.ViewModels;

public class WebhookViewModel : ReactiveObject
{
    readonly DataFacade _dataFacade;
    readonly ObservableAsPropertyHelper<List<WebhookChatEntity>> _listWebhookChat;

    [Reactive]
    public LoadingConfiguration Loading { get; set; } = new();

    public List<WebhookChatEntity> ListWebhookChat => _listWebhookChat.Value;

    public ReactiveCommand<Unit, List<WebhookChatEntity>> LoadDataCommand { get; }

    public WebhookViewModel(DataFacade dataFacade)
    {
        _dataFacade = dataFacade;

        Loading.TotalSteps = 2;

        LoadDataCommand = ReactiveCommand.CreateFromTask(LoadData);

        _listWebhookChat = LoadDataCommand.ToProperty(this, x => x.ListWebhookChat, scheduler: RxApp.MainThreadScheduler);
    }

    private async Task<List<WebhookChatEntity>> LoadData()
    {
        Loading.CurrentStep = 1;

        var data = await _dataFacade.MongoDb.WebhookChat
            .Where(x => x.Status == EventStatus.Complete)
            .ToListAsync();

        Loading.CurrentStep = 2;

        return data;
    }
}