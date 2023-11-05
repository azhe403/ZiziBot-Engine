using System.ComponentModel;
using Microsoft.AspNetCore.Components.Authorization;

namespace ZiziBot.Console.Partials;

public class WebComponentBase<T> : ReactiveInjectableComponentBase<T>  where T : class, INotifyPropertyChanged
{
    [Inject]
    public required IMediator Mediator { get; set; }

    [Inject]
    public required ILocalStorageService LocalStorage { get; set; }

    [Inject]
    public required ProtectedLocalStorage ProtectedLocalStorage { get; set; }

    [Inject]
    public required MongoDbContextBase MongoDbContextBase { get; set; }

    [Inject]
    public required ILogger<T> Logger { get; set; }

    [CascadingParameter]
    public Task<AuthenticationState>? AuthenticationState { get; set; }
}