using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace ZiziBot.Console.Pages
{
    public partial class User
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

        // [Inject]
        // protected AppMongoDbContext AppMongoDbContext { get; set; }

        // UserEntity userEntity = new();

        protected async Task OnSaveUser(MouseEventArgs args)
        {
            await JSRuntime.InvokeVoidAsync("console.log", "P");
            System.Console.WriteLine("P");

            // AppMongoDbContext.User.Add(new UserEntity()
            // {
            //     Name = userEntity.Name
            // });
            //
            // await AppMongoDbContext.SaveChangesAsync();
        }
    }
}