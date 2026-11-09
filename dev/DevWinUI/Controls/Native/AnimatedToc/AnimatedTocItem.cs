using System.ComponentModel;

namespace DevWinUI;

public sealed partial class AnimatedTocItem : INotifyPropertyChanged
{
    private string _label = string.Empty;
    private string _href = string.Empty;
    private int _depth;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Label
    {
        get => _label;
        set => Set(ref _label, value ?? string.Empty, nameof(Label));
    }

    public int Depth
    {
        get => _depth;
        set => Set(ref _depth, value, nameof(Depth));
    }

    public string Href
    {
        get => _href;
        set => Set(ref _href, value ?? string.Empty, nameof(Href));
    }

    private void Set<T>(ref T field, T value, string name)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
