using MongoDB.Bson;
using ZiziBot.Application.Services;
using ZiziBot.DataSource.MongoDb.Entities;
using Unit = System.Reactive.Unit;

namespace ZiziBot.Console.ViewModels;

public class NoteAddViewModel : ReactiveObject, IActivatableViewModel
{
    private readonly DialogService _dialogService;
    private readonly NoteService _noteService;
    public ViewModelActivator Activator { get; } = new ViewModelActivator();
    public ReactiveCommand<Unit, Unit> OnSaveCommand { get; }
    public ReactiveCommand<Unit, Unit> OnCancelCommand { get; }

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
    public string RawButton { get; set; }

    [Reactive]
    public DateTime CreatedDate { get; set; }

    [Reactive]
    public DateTime UpdatedDate { get; set; }

    public NoteAddViewModel(DialogService dialogService, NoteService noteService)
    {
        _dialogService = dialogService;
        _noteService = noteService;

        OnSaveCommand = ReactiveCommand.CreateFromTask(() => SaveCommand());
        OnCancelCommand = ReactiveCommand.CreateFromTask(() => CancelCommand());
    }

    private async Task SaveCommand()
    {
        await _noteService.Save(new NoteEntity()
        {
            Id = ObjectId.Parse(NoteId),
            ChatId = ChatId,
            Query = Query,
            Content = Content,
            RawButton = RawButton,
            FileId = FileId,
            TransactionId = Guid.NewGuid().ToString()
        });

        _dialogService.Close();
    }

    private async Task CancelCommand()
    {
        _dialogService.Close();
    }
}