using System.ComponentModel;

namespace ZiziBot.Types.Types;

public sealed class ConsoleMenuModel : INotifyPropertyChanged
{
    private bool _expanded;
    private readonly Func<List<ConsoleMenuModel>> _collection;

    public string? Text { get; init; }
    public string? Icon { get; init; }
    public string? Path { get; init; }

    public IEnumerable<ConsoleMenuModel>? Items { get; init; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ConsoleMenuModel(Func<List<ConsoleMenuModel>> collection)
    {
        this._collection = collection;
    }

    public bool Expanded
    {
        get => _expanded;
        set
        {
            if (_expanded == value) return;

            _collection().Where(i => i != this).ToList().ForEach(s => s.Expanded = false);

            _expanded = value;
            OnPropertyChanged(nameof(Expanded));
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}