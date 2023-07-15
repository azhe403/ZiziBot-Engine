using Microsoft.AspNetCore.Components.Routing;

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

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            NavigationManager.LocationChanged += NavigationManagerOnLocationChanged;
            return base.OnAfterRenderAsync(firstRender);
        }

        private void NavigationManagerOnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            // throw new NotImplementedException();
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