using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DevWinUI;

public sealed partial class SidebarViewSampleItem : ISidebarItemModel, ISidebarItemPresentationModel
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private string? _text;
    public string? Text
    {
        get => _text;
        set => Set(ref _text, value);
    }

    private object? _toolTip;
    public object? ToolTip
    {
        get => _toolTip ?? _text;
        set => Set(ref _toolTip, value);
    }

    private IconSource? _iconSource;
    public IconSource? IconSource
    {
        get => _iconSource;
        set
        {
            if (ReferenceEquals(_iconSource, value))
                return;

            _iconSource = value;
            Notify(nameof(IconSource));
            Notify(nameof(IconElement));
        }
    }

    public Symbol? IconSymbol
    {
        set => IconSource = value is { } symbol ? new SymbolIconSource { Symbol = symbol } : null;
    }

    public string? IconImage
    {
        set => IconSource = value is null ? null : new ImageIconSource
        {
            ImageSource = new BitmapImage(new Uri($"ms-appx:///Assets/Fluent/{value}.png"))
        };
    }

    public FrameworkElement? IconElement => _iconSource?.CreateIconElement();

    private FrameworkElement? _itemDecorator;
    public FrameworkElement? ItemDecorator
    {
        get => _itemDecorator;
        set => Set(ref _itemDecorator, value);
    }

    private ObservableCollection<SidebarViewSampleItem>? _children;
    public ObservableCollection<SidebarViewSampleItem>? Children
    {
        get => _children;
        set => Set(ref _children, value);
    }

    object? ISidebarItemModel.Children => _children;

    private bool _isExpanded = true;
    public bool IsExpanded
    {
        get => _isExpanded;
        set => Set(ref _isExpanded, value);
    }

    private bool _hasUnrealizedChildren;
    public bool HasUnrealizedChildren
    {
        get => _hasUnrealizedChildren;
        set => Set(ref _hasUnrealizedChildren, value);
    }

    private bool _isLeafWithChildren;
    public bool IsLeafWithChildren
    {
        get => _isLeafWithChildren;
        set => Set(ref _isLeafWithChildren, value);
    }

    private string? _path;
    public string? Path
    {
        get => _path;
        set => Set(ref _path, value);
    }

    private void Notify(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return;

        field = value;
        Notify(name!);
    }
}
