using System.Net;
using Microsoft.AspNetCore.Components.Authorization;

namespace ZiziBot.Console.Shared
{
    public partial class MainLayout
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
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        protected ProtectedLocalStorage ProtectedLocalStorage { get; set; }

        [Inject]
        protected CustomAuthenticationStateProvider CustomAuthenticationStateProvider { get; set; }

        [Inject]
        public IMediator Mediator { get; set; }

        [Inject]
        public ILogger<MainLayout> Logger { get; set; }

        private bool sidebarExpanded = true;

        private void SidebarToggleClick()
        {
            sidebarExpanded = !sidebarExpanded;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!firstRender)
                return;

            var sessionDto = NavigationManager.QueryString<TelegramSessionDto>();
            var validate = await sessionDto?.ValidateAsync<TelegramSessionDtoValidator, TelegramSessionDto>();
            if (validate.IsValid)
            {
                var webResponse = await Mediator.Send(new CheckConsoleSessionRequest()
                {
                    Model = sessionDto
                });

                if (webResponse.StatusCode != HttpStatusCode.OK)
                {
                    NotificationService.Notify(NotificationSeverity.Warning, "Sesi tidak valid, Silahkan coba lagi");
                    Logger.LogDebug("Session is invalid, continue..");
                    return;
                }

                await ProtectedLocalStorage.SetAsync("bearer_token", webResponse.Result?.BearerToken);
                NotificationService.Notify(NotificationSeverity.Success,
                    $"Selamat datang, {sessionDto.FirstName} {sessionDto.LastName}");

                await JSRuntime.InvokeVoidAsync("eval",
                    $"document.cookie='bearer_token={webResponse.Result?.BearerToken}; path=/hangfire-jobs; expires=Fri, 31 Dec 2024;'");
                NavigationManager.NavigateTo("/", replace: true);
            }

            Logger.LogDebug("URL contains no session payload, continue..");
            await ValidateBearer();
        }

        private void OnParentClicked(MenuItemEventArgs args)
        {
            Logger.LogDebug($"{args.Text} clicked from parent");
        }

        private void OnChildClicked(MenuItemEventArgs args)
        {
            Logger.LogDebug($"{args.Text} from child clicked");
        }

        private async Task ValidateBearer()
        {
            var bearerToken = await ProtectedLocalStorage.GetAsync<string>("bearer_token");

            if (bearerToken.Value == null)
            {
                Logger.LogWarning("Bearer empty");

                return;
            }

            ((CustomAuthenticationStateProvider)AuthenticationStateProvider).AuthenticateUser(bearerToken.Value);
        }
    }
}