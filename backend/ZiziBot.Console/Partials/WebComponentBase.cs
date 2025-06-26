using System.ComponentModel;
using Microsoft.AspNetCore.Components.Authorization;
using ZiziBot.Database.MongoDb;

namespace ZiziBot.Console.Partials;

public class WebComponentBase<T> : ReactiveInjectableComponentBase<T> where T : class, INotifyPropertyChanged
{
    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    [Inject]
    public required IMediator Mediator { get; set; }

    [Inject]
    public required ILocalStorageService LocalStorage { get; set; }

    [Inject]
    public required ProtectedLocalStorage ProtectedLocalStorage { get; set; }

    [Inject]
    public required MongoDbContext MongoDbContext { get; set; }

    [Inject]
    public required ILogger<T> Logger { get; set; }

    [CascadingParameter]
    public Task<AuthenticationState>? AuthenticationState { get; set; }

    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    protected NotificationService NotificationService { get; set; }
}