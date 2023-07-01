using System.Collections.Specialized;
using System.Net;
using Blazored.LocalStorage;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Radzen;

namespace ZiziBot.Console.Pages
{
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
        public ILogger<Index> Logger { get; set; }

        [Parameter]
        [SupplyParameterFromQuery(Name = "session_id")]
        public string SessionId { get; set; }


        public NameValueCollection QueryParams => NavigationManager.QueryString();

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }

        protected override Task OnParametersSetAsync()
        {
            return base.OnParametersSetAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var sessionDto = NavigationManager.QueryString<TelegramSessionDto>();
            var validate = await sessionDto?.ValidateAsync<TelegramSessionDtoValidator, TelegramSessionDto>();
            if (!validate.IsValid)
            {
                Logger.LogDebug("URL contains no session payload, continue..");
                return;
            }

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

            await LocalStorage.SetItemAsStringAsync("bearer_token", webResponse.Result?.BearerToken);
            NotificationService.Notify(NotificationSeverity.Success, $"Selamat datang, {sessionDto.FirstName} {sessionDto.LastName}");

            await JSRuntime.InvokeVoidAsync("eval",$"document.cookie='bearer_token={webResponse.Result?.BearerToken}; path=/hangfire-jobs; expires=Fri, 31 Dec 2024;'");
            NavigationManager.NavigateTo("/", replace: true);

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}