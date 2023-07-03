using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Radzen;

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
        public ILogger<MainLayout> Logger { get; set; }

        private bool sidebarExpanded = true;

        void SidebarToggleClick()
        {
            sidebarExpanded = !sidebarExpanded;
        }

        void OnParentClicked(MenuItemEventArgs args)
        {
            Logger.LogDebug($"{args.Text} clicked from parent");
        }

        void OnChildClicked(MenuItemEventArgs args)
        {
            Logger.LogDebug($"{args.Text} from child clicked");
        }
    }
}