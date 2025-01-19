using ZiziBot.Application.Facades;

namespace ZiziBot.Console.Partials;

public partial class ChatSelector : WebComponentBase<ChatSelectorViewModel>
{
    [Inject]
    protected DataFacade DataFacade { get; set; }

    [Parameter]
    public SelectorMode SelectorMode { get; set; }

    [Parameter]
    public long SelectedChatId { get; set; }

    [Parameter]
    public List<ChatInfoDto>? ListChat { get; set; }

    [Parameter]
    public EventCallback<long> SelectedChatIdChanged { get; set; }

    private async Task OnValueChanged(object obj)
    {
        if (obj is not long chatId) return;

        await SelectedChatIdChanged.InvokeAsync(chatId);
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnLoadData(LoadDataArgs obj)
    {
        var bearerToken = await ProtectedLocalStorage.GetAsync<string>("bearer_token");
        if (bearerToken.Value == null)
            return;

        ListChat = await DataFacade.ChatSetting.GetChatByBearerToken(bearerToken.Value);
        if (!ListChat.IsEmpty() && SelectedChatId == 0)
        {
            SelectedChatId = ListChat.FirstOrDefault()?.ChatId ?? 0;
            if (SelectedChatId != 0)
                await SelectedChatIdChanged.InvokeAsync(SelectedChatId);
        }

        await InvokeAsync(StateHasChanged);
    }
}

public enum SelectorMode
{
    List,
    Dropdown,
    Tiles
}