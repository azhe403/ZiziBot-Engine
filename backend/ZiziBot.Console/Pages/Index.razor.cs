using Microsoft.AspNetCore.Components.Authorization;

namespace ZiziBot.Console.Pages;

public partial class Index
{
    [Inject]
    protected IJSRuntime JSRuntime { get; set; }

    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    protected DialogService DialogService { get; set; }

    [Inject]
    protected TooltipService TooltipService { get; set; }

    [Inject]
    protected ContextMenuService ContextMenuService { get; set; }

    [Inject]
    protected NotificationService NotificationService { get; set; }

    [Inject]
    public IMediator Mediator { get; set; }

    [Inject]
    public ILocalStorageService LocalStorage { get; set; }

    [Inject]
    protected ProtectedLocalStorage ProtectedLocalStorage { get; set; }

    [Inject]
    protected CustomAuthenticationStateProvider CustomAuthenticationStateProvider { get; set; }

    [Inject]
    protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public ILogger<Index> Logger { get; set; }

    [Parameter]
    [SupplyParameterFromQuery(Name = "session_id")]
    public string SessionId { get; set; }

}