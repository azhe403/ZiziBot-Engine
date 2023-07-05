namespace ZiziBot.Console.Partials;

public partial class ChatSelector: ReactiveInjectableComponentBase<ChatSelectorViewModel>
{
    [Inject]
    protected IMediator Mediator { get; set; }

    [Inject]
    protected ILocalStorageService LocalStorage { get; set; }

    [Inject]
    protected ProtectedLocalStorage ProtectedLocalStorage { get; set; }

    [Inject]
    protected ChatDbContext ChatDbContext { get; set; }

    [Inject]
    protected ChatSettingRepository ChatSettingRepository { get; set; }

    [Inject]
    protected ILogger<ChatSelector> Logger { get; set; }

    [Parameter]
    public long SelectedChatId { get; set; }

    [Parameter]
    public List<ChatInfoDto>? ListChat { get; set; }

    [Parameter]
    public Action<long> OnChatSelected { get; set; }

    [Parameter]
    public EventCallback<long> OnChatSelectedCallback { get; set; }

    private async Task OnValueChanged(object obj)
    {
        if (obj is not long chatId) return;

        await OnChatSelectedCallback.InvokeAsync(chatId);
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnLoadData(LoadDataArgs obj)
    {
        var bearerToken = await ProtectedLocalStorage.GetAsync<string>("bearer_token");
        if (bearerToken.Value == null)
            return;

        ListChat = await ChatSettingRepository.GetChatByBearerToken(bearerToken.Value);
        if(!ListChat.IsEmpty())
        {
            SelectedChatId = ListChat.FirstOrDefault().ChatId;
            await OnChatSelectedCallback.InvokeAsync(SelectedChatId);
        }

        await InvokeAsync(StateHasChanged);
    }
}