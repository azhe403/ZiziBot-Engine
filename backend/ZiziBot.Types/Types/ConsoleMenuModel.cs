using System.ComponentModel;

namespace ZiziBot.Types.Types;

public class ConsoleMenuModel : INotifyPropertyChanged
{
    private bool _expanded;

    public string Text { get; set; }
    public IEnumerable<ConsoleMenuModel> Items { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    private readonly Func<List<ConsoleMenuModel>> _collection;

    public ConsoleMenuModel(Func<List<ConsoleMenuModel>> collection)
    {
        this._collection = collection;
    }

    public bool Expanded
    {
        get { return _expanded; }
        set
        {
            if (_expanded == value) return;

            _collection()?.Where(i => i != this).ToList().ForEach(s => s.Expanded = false);

            _expanded = value;
            OnPropertyChanged(nameof(Expanded));
        }
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}