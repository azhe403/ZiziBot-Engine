namespace ZiziBot.Console.ViewModels;

public class ChatSelectorViewModel : ReactiveObject
{
    public ViewModelActivator Activator { get; } = new();

    [Reactive]
    public long SelectedChatId { get; set; }

    [Reactive]
    public List<ChatInfoDto>? ListChat { get; set; }

    public ChatSelectorViewModel()
    {

    }
}