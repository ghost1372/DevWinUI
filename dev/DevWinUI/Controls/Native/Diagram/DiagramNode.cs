using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[ContentProperty(Name = nameof(Children))]
public sealed partial class DiagramNode : INotifyPropertyChanged
{
    private string? _id;
    private string? _type;
    private string? _title;
    private string? _caption;
    private object? _data;
    private Color _hue = Color.FromArgb(255, 0x5B, 0x8D, 0xEF);
    private int _row;
    private double _x = 0.5;
    private double _w;
    private double _h;
    private bool _expanded = true;
    private bool _selectable = true;
    private string? _glyph;
    private string? _subtitle;
    private double _canvasX;
    private double _canvasY;

    public DiagramNode()
    {
        Children = new DiagramNodeCollection(this);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    internal event EventHandler? Changed;

    public string? Id
    {
        get => _id;
        set => Set(ref _id, value);
    }

    public string? Type
    {
        get => _type;
        set => Set(ref _type, value, true);
    }

    public string? Title
    {
        get => _title;
        set => Set(ref _title, value, true);
    }

    public string? Caption
    {
        get => _caption;
        set => Set(ref _caption, value, true);
    }

    public object? Data
    {
        get => _data;
        set => Set(ref _data, value, true);
    }

    public Color Hue
    {
        get => _hue;
        set => Set(ref _hue, value);
    }

    public int Row
    {
        get => _row;
        set => Set(ref _row, value);
    }

    public double X
    {
        get => _x;
        set => Set(ref _x, value);
    }

    public double W
    {
        get => _w;
        set => Set(ref _w, value, true);
    }

    public double H
    {
        get => _h;
        set => Set(ref _h, value, true);
    }

    public bool Expanded
    {
        get => _expanded;
        set => Set(ref _expanded, value);
    }

    public bool Selectable
    {
        get => _selectable;
        set => Set(ref _selectable, value);
    }

    public string? Glyph
    {
        get => _glyph;
        set => Set(ref _glyph, value, true);
    }

    public string? Subtitle
    {
        get => _subtitle;
        set => Set(ref _subtitle, value, true);
    }

    public double CanvasX
    {
        get => _canvasX;
        set => Set(ref _canvasX, value);
    }

    public double CanvasY
    {
        get => _canvasY;
        set => Set(ref _canvasY, value);
    }

    public List<DiagramPin> Inputs { get; } = new();

    public List<DiagramPin> Outputs { get; } = new();

    public bool HasPins => Inputs.Count > 0 || Outputs.Count > 0;

    public ObservableCollection<DiagramNode> Children { get; }

    public List<DiagramCondition> Conditions { get; } = new();

    public string DisplayText => Title ?? Data?.ToString() ?? Id ?? string.Empty;

    public double Left { get; internal set; }

    public double Top { get; internal set; }

    internal double OffsetX { get; set; }

    internal double OffsetY { get; set; }

    internal double CachedHeight { get; set; }

    internal int CacheEpoch { get; set; }

    internal void SetCanvas(double x, double y)
    {
        _canvasX = x;
        _canvasY = y;
    }

    private bool Set<T>(ref T field, T value, bool resize = false, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        if (resize)
        {
            CachedHeight = 0;
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        if (name is nameof(Title) or nameof(Data) or nameof(Id))
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayText)));
        }

        Changed?.Invoke(this, EventArgs.Empty);
        return true;
    }

    private void OnChildChanged(object? sender, EventArgs e) => Changed?.Invoke(this, EventArgs.Empty);

    internal void Attach(DiagramNode child)
    {
        child.Changed += OnChildChanged;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    internal void Detach(DiagramNode child)
    {
        child.Changed -= OnChildChanged;
        Changed?.Invoke(this, EventArgs.Empty);
    }
}
