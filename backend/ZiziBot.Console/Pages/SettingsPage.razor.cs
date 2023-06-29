using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Radzen;

namespace ZiziBot.Console.Pages
{
    public partial class SettingsPage
    {
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

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
        protected ILogger<SettingsPage> Logger { get; set; }

        [Inject]
        protected ILocalStorageService LocalStorageService { get; set; }

        protected async Task AnuChange(bool args)
        {
            Logger.LogInformation("CheckBox: {args}", args);
            NotificationService.Notify(NotificationSeverity.Info, "CheckBox", "Lorem ipsum dolor!", click: message => {
                Logger.LogDebug("Clicked!");
            });

            await LocalStorageService.SetItemAsync("checkbox", args);

        }

        private async Task DropDown0Change(System.Object args)
        {
        }
    }
}