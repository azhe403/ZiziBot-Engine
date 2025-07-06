using System.Reactive.Linq;
using Microsoft.AspNetCore.Components.Web;
using ZiziBot.Common.Dtos;
using ZiziBot.Database.Repository;
using Unit = System.Reactive.Unit;

namespace ZiziBot.Console.ViewModels;

public class BadwordViewModel : ReactiveObject, IActivatableViewModel
{
    private readonly WordFilterRepository _wordFilterRepository;

    public ViewModelActivator Activator { get; }

    [Reactive]
    public bool HasErrors { get; private set; }

    [Reactive]
    public string Word { get; set; } = string.Empty;

    [Reactive]
    public List<BadwordDto>? Words { get; set; }

    public ReactiveCommand<KeyboardEventArgs, Unit> OnEnterCommand { get; }
    public ReactiveCommand<Unit, Unit> OnSaveCommand { get; }

    public BadwordViewModel(WordFilterRepository wordFilterRepository)
    {
        _wordFilterRepository = wordFilterRepository;

        var isValidObs = this.WhenAnyValue(m => m.Word, word => !string.IsNullOrEmpty(word))
            .Log(this, "Validity changed")
            .Publish()
            .RefCount();

        isValidObs.Subscribe(hasError => HasErrors = !hasError);

        OnEnterCommand = ReactiveCommand.Create<KeyboardEventArgs>(HandleEnter, isValidObs);
        OnSaveCommand = ReactiveCommand.CreateFromTask(SaveWord, isValidObs);
    }

    private void HandleEnter(KeyboardEventArgs obj)
    {
        if (obj.Key == "Enter")
        {
        }
    }

    private async Task SaveWord()
    {
        await _wordFilterRepository.SaveAsync(new WordFilterDto() {
            ChatId = 1,
            UserId = 1,
            Word = Word,
            IsGlobal = true,
            TransactionId = Guid.NewGuid().ToString()
        });

        Word = string.Empty;

        await LoadData();
    }

    public async Task LoadData()
    {
        var findWordFilter = await _wordFilterRepository.GetAllAsync();

        Words = findWordFilter.Select(x => new BadwordDto() {
                Id = x.Id,
                Word = x.Word
            })
            .ToList();
    }
}

public class BadwordDto
{
    public string Id { get; set; }
    public string Word { get; set; }
}