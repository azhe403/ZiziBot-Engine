namespace ZiziBot.Console.Pages;

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
    protected long ChatId { get; set; }
    protected List<NoteDto> ListNote { get; set; }

    private async Task OnSelectChatCallback(long chatId)
    {
        Logger.LogDebug("Selected Chat: {ChatId}", chatId);
        ChatId = chatId;

        await LoadNotes();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnSelectedItemChanged(object obj)
    {
        if (obj is not string noteId) return;

        await DialogService.OpenAsync<NoteAdd>(
            title: "Edit Note",
            parameters: new()
            {
                { "NoteId", noteId }
            },
            options: new()
            {
                Width = "700px",
                CloseDialogOnEsc = true,
                Resizable = true,
                Draggable = true
            });

        await LoadNotes();
    }

    private async Task OnOpenAddDialog()
    {
        await DialogService.OpenAsync<NoteAdd>(
            title: "Add Note",
            options: new()
            {
                Width = "700px",
                CloseDialogOnEsc = true,
                Resizable = true,
                Draggable = true
            });

        await LoadNotes();
    }

    private async Task LoadNotes()
    {
        ListNote = await ChatSettingRepository.GetListNote(ChatId);
    }
}