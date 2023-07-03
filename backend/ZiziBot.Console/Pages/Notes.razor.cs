using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Radzen;

namespace ZiziBot.Console.Pages
{
    public partial class Notes
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
        protected ChatSettingRepository ChatSettingRepository { get; set; }

        [Inject]
        protected ILogger<Notes> Logger { get; set; }

        protected string noteId { get; set; }
        protected List<NoteDto> ListNote { get; set; }

        private async Task OnSelectChatCallback(long chatId)
        {
            Logger.LogDebug("Selected Chat: {ChatId}", chatId);

            ListNote = await ChatSettingRepository.GetListNote(chatId);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}