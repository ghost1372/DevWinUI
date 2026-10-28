namespace DevWinUI;

public sealed partial class BranchedMenu
{
    public string? SelectedValue => _active?.Value;

    public event EventHandler<BranchedMenuSelectedEventArgs>? ItemSelected;
    public event EventHandler<BranchedMenuToggledEventArgs>? SectionToggled;

    public IList<BranchedMenuItem> Items
    {
        get { return (IList<BranchedMenuItem>)GetValue(ItemsProperty); }
        set { SetValue(ItemsProperty, value); }
    }

    public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register(nameof(Items), typeof(IList<BranchedMenuItem>), typeof(BranchedMenu), new PropertyMetadata(null, OnRebuildChanged));

    public int[] DefaultOpen
    {
        get { return (int[])GetValue(DefaultOpenProperty); }
        set { SetValue(DefaultOpenProperty, value); }
    }

    public static readonly DependencyProperty DefaultOpenProperty =
        DependencyProperty.Register(nameof(DefaultOpen), typeof(int[]), typeof(BranchedMenu), new PropertyMetadata(new[] { 0 }, OnRebuildChanged));

    public string DefaultActive
    {
        get { return (string)GetValue(DefaultActiveProperty); }
        set { SetValue(DefaultActiveProperty, value); }
    }

    public static readonly DependencyProperty DefaultActiveProperty =
        DependencyProperty.Register(nameof(DefaultActive), typeof(string), typeof(BranchedMenu), new PropertyMetadata("", OnRebuildChanged));

    public Brush? InkBrush
    {
        get { return (Brush?)GetValue(InkBrushProperty); }
        set { SetValue(InkBrushProperty, value); }
    }

    public static readonly DependencyProperty InkBrushProperty =
        DependencyProperty.Register(nameof(InkBrush), typeof(Brush), typeof(BranchedMenu), new PropertyMetadata(null, OnRebuildChanged));

    public Brush? AccentBrush
    {
        get { return (Brush?)GetValue(AccentBrushProperty); }
        set { SetValue(AccentBrushProperty, value); }
    }

    public static readonly DependencyProperty AccentBrushProperty =
        DependencyProperty.Register(nameof(AccentBrush), typeof(Brush), typeof(BranchedMenu), new PropertyMetadata(null, OnRebuildChanged));

    public Brush? LineBrush
    {
        get { return (Brush?)GetValue(LineBrushProperty); }
        set { SetValue(LineBrushProperty, value); }
    }

    public static readonly DependencyProperty LineBrushProperty =
        DependencyProperty.Register(nameof(LineBrush), typeof(Brush), typeof(BranchedMenu), new PropertyMetadata(null, OnRebuildChanged));

    public double MenuMaxWidth
    {
        get { return (double)GetValue(MenuMaxWidthProperty); }
        set { SetValue(MenuMaxWidthProperty, value); }
    }

    public static readonly DependencyProperty MenuMaxWidthProperty =
        DependencyProperty.Register(nameof(MenuMaxWidth), typeof(double), typeof(BranchedMenu), new PropertyMetadata(240.0, OnRebuildChanged));

    public double RowHeight
    {
        get { return (double)GetValue(RowHeightProperty); }
        set { SetValue(RowHeightProperty, value); }
    }

    public static readonly DependencyProperty RowHeightProperty =
        DependencyProperty.Register(nameof(RowHeight), typeof(double), typeof(BranchedMenu), new PropertyMetadata(36.0, OnRebuildChanged));

    public double Indent
    {
        get { return (double)GetValue(IndentProperty); }
        set { SetValue(IndentProperty, value); }
    }

    public static readonly DependencyProperty IndentProperty =
        DependencyProperty.Register(nameof(Indent), typeof(double), typeof(BranchedMenu), new PropertyMetadata(40.0, OnRebuildChanged));

    public double Trunk
    {
        get { return (double)GetValue(TrunkProperty); }
        set { SetValue(TrunkProperty, value); }
    }

    public static readonly DependencyProperty TrunkProperty =
        DependencyProperty.Register(nameof(Trunk), typeof(double), typeof(BranchedMenu), new PropertyMetadata(14.0, OnRebuildChanged));

    public double Radius
    {
        get { return (double)GetValue(RadiusProperty); }
        set { SetValue(RadiusProperty, value); }
    }

    public static readonly DependencyProperty RadiusProperty =
        DependencyProperty.Register(nameof(Radius), typeof(double), typeof(BranchedMenu), new PropertyMetadata(10.0, OnRebuildChanged));

    public double LineWidth
    {
        get { return (double)GetValue(LineWidthProperty); }
        set { SetValue(LineWidthProperty, value); }
    }

    public static readonly DependencyProperty LineWidthProperty =
        DependencyProperty.Register(nameof(LineWidth), typeof(double), typeof(BranchedMenu), new PropertyMetadata(1.5, OnRebuildChanged));

    public int DrawDuration
    {
        get { return (int)GetValue(DrawDurationProperty); }
        set { SetValue(DrawDurationProperty, value); }
    }

    public static readonly DependencyProperty DrawDurationProperty =
        DependencyProperty.Register(nameof(DrawDuration), typeof(int), typeof(BranchedMenu), new PropertyMetadata(400));

    public int FoldDuration
    {
        get { return (int)GetValue(FoldDurationProperty); }
        set { SetValue(FoldDurationProperty, value); }
    }

    public static readonly DependencyProperty FoldDurationProperty =
        DependencyProperty.Register(nameof(FoldDuration), typeof(int), typeof(BranchedMenu), new PropertyMetadata(300));

    private static void OnRebuildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((BranchedMenu)d).Rebuild();
}
