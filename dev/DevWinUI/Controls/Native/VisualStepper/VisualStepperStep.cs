using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DevWinUI;

public sealed partial class VisualStepperStep : INotifyPropertyChanged
{
    private string _title = "";
    private string? _description;
    private object? _content;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Title
    {
        get => _title;
        set => Set(ref _title, value);
    }

    public string? Description
    {
        get => _description;
        set => Set(ref _description, value);
    }

    public object? Content
    {
        get => _content;
        set => Set(ref _content, value);
    }

    private void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
