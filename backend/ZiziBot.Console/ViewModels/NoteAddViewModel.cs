using System.Reactive.Linq;
using Unit = System.Reactive.Unit;

namespace ZiziBot.Console.ViewModels;

public class NoteAddViewModel : ReactiveObject, IActivatableViewModel
{
    private readonly IMediator _mediator;
    private readonly DialogService _dialogService;
    public ViewModelActivator Activator { get; } = new ViewModelActivator();
    public ReactiveCommand<Unit, Unit> OnSaveCommand { get; }
    public ReactiveCommand<Unit, Unit> OnCancelCommand { get; }

    [Reactive]
    public bool HasErrors { get; private set; }

    [Reactive]
    public string NoteId { get; set; }

    [Reactive]
    public long ChatId { get; set; }

    [Reactive]
    public string Query { get; set; }

    [Reactive]
    public string Content { get; set; }

    [Reactive]
    public string FileId { get; set; }

    [Reactive]
    public int MediaTypeId { get; set; }

    [Reactive]
    public string RawButton { get; set; }

    [Reactive]
    public DateTime CreatedDate { get; set; }

    [Reactive]
    public DateTime UpdatedDate { get; set; }

    public NoteAddViewModel(IMediator mediator, DialogService dialogService)
    {
        _mediator = mediator;
        _dialogService = dialogService;

        var valid = this.WhenAnyValue(vm => vm.Query, (model) => !string.IsNullOrEmpty(model))
            .Log(this, "Validity changed")
            .Publish()
            .RefCount();

        valid.Subscribe(x => HasErrors = !x);

        OnSaveCommand = ReactiveCommand.CreateFromTask(() => SaveCommand(), canExecute: valid);

        OnCancelCommand = ReactiveCommand.CreateFromTask(() => CancelCommand());
    }

    private async Task SaveCommand()
    {
        await _mediator.Send(new SaveNoteRequest() {
            Id = NoteId,
            ChatId = ChatId,
            Query = Query,
            Content = Content,
            RawButton = RawButton,
            FileId = FileId,
            DataType = MediaTypeId
        });

        _dialogService.Close();
    }

    private async Task CancelCommand()
    {
        _dialogService.Close();
    }
}